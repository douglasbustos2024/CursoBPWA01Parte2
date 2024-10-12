using Empresa.Inv.Application.Shared;
using Empresa.Inv.Application;
 
using Empresa.Inv.EntityFrameworkCore;
using Empresa.Inv.EntityFrameworkCore.Repositories;
 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Cryptography;
using Microsoft.OpenApi.Models;
 
using Empresa.Inv.Application.Shared.Entities.Dtos;
using Empresa.Inv.Application.Shared.Entities;
using Empresa.Inv.HttpApi.Services;
using Empresa.Inv.Infraestructure;
using WebAppApiArq.Data;
using Empresa.Inv.Dtos;
using Empresa.Inv.Application.Validators;
using FluentValidation.AspNetCore;
using FluentValidation;
using System.Reflection;
using Azure.Identity;

namespace Empresa.Inv.Web.Host
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
                                                   
            // Configuración CORS
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });




            var keyVaultUrl = builder.Configuration["KeyVaultUrl"] ?? string.Empty;

            if (builder.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "local")
            {
                string tenantId = Environment.GetEnvironmentVariable("tenantId") ?? string.Empty;
                string clientId = Environment.GetEnvironmentVariable("clientId") ?? string.Empty;
                string clientSecret = Environment.GetEnvironmentVariable("clientSecret") ?? string.Empty;

                ClientSecretCredential tokenCredentials;

                try
                {
                    tokenCredentials = new ClientSecretCredential(tenantId, clientId, clientSecret);
                    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl), tokenCredentials);
                }
                catch (Exception ee)
                {
                    Log.Fatal(ee, "La aplicación falló al iniciar.");
                }
            }
            else
            {
                builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());
            }


            // Configurar mapeo de secciones del appsettings.json
            builder.Services.Configure<SensitiveSettings>(builder.Configuration.GetSection("SensitiveSettings"));


            // Obtener valores de secretos desde Key Vault
            var privateKey      = builder.Configuration["JwtSettings-PrivateKey"];
            var publicKey       = builder.Configuration["JwtSettings-PublicKey"];
            var smtpUsername    = builder.Configuration["EmailSettings-Username"];
            var smtpPassword    = builder.Configuration["EmailSettings-Password"];                                
                                                                                           

            // Validar que los secretos se cargaron correctamente
            if (string.IsNullOrWhiteSpace(privateKey) || string.IsNullOrWhiteSpace(publicKey))
            {
                throw new InvalidOperationException("Las claves JWT (privada o pública) no se pudieron cargar correctamente desde Azure Key Vault.");
            }

            if (string.IsNullOrWhiteSpace(smtpUsername) || string.IsNullOrWhiteSpace(smtpPassword))
            {
                throw new InvalidOperationException("Las credenciales de correo electrónico no se pudieron cargar correctamente desde Azure Key Vault.");
            }


            //// Configuración JWT
    

            // Cargar secretos de Key Vault (claves privadas y públicas)
            var jwtSettings = new  JwtSettingsFile
            {
                Issuer = builder.Configuration["JwtSettings:Issuer"],
                Audience = builder.Configuration["JwtSettings:Audience"],
                ExpiresInMinutes = int.Parse(builder.Configuration["JwtSettings:ExpiresInMinutes"]),
                PrivateKey = builder.Configuration["JwtSettings-PrivateKey"],  // Se carga el contenido de la clave privada
                PublicKey = builder.Configuration["JwtSettings-PublicKey"]    // Se carga el contenido de la clave pública
            };


            // Configuración de TwoFactorSettings
            builder.Services.Configure<TwoFactorSettings>(builder.Configuration.GetSection("TwoFactorAuthentication"));

            // Registro de servicios de configuración
             builder.Services.AddSingleton(jwtSettings);


            #region Monitoreo

            // Añade Application Insights a la aplicación
            builder.Services.AddApplicationInsightsTelemetry();

            #endregion











            //// Cargar configuración de appsettings.json
  

            // Registro de servicios
            builder.Services.AddScoped<JwtTokenService>();
            builder.Services.AddTransient<IEmailSender, EmailSender>(); // Servicio para enviar correos electrónicos

            // Registro de autorización personalizada
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("PermissionPolicy", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("")); // Se establecerá en el atributo
                });
            });

            // Autenticación JWT
            var privateKeyContent = jwtSettings.PrivateKey;
          

            RSA rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyContent.ToCharArray());
            var rsaSecurityKey = new RsaSecurityKey(rsa);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = rsaSecurityKey,
                    ClockSkew = TimeSpan.Zero // Elimina el margen de 5 minutos en la expiración de tokens
                };
            });

 


            // Configurar Serilog para usar Application Insights con ConnectionString
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)  // Leer la configuración desde appsettings.json
                .WriteTo.ApplicationInsights(builder.Configuration["ApplicationInsights-ConnectionStringNew"],
                    TelemetryConverter.Traces)  // Convertir los logs a trazas en AI
                .CreateLogger();




            // Agrega un registro de prueba
            Log.Information("Aplicación iniciada.");

            try
            {
                // Usa Serilog como el logger
                builder.Host.UseSerilog();

                // Agregar controladores
                builder.Services.AddControllers();

                #region Validaciones con FluentValidation

                builder.Services.AddFluentValidationAutoValidation();
                builder.Services.AddTransient<IValidator<ProductDto>, ProductValidator>();

                #endregion


  
                                                    
               

                #region KeyVault - Cadena de conexion base de datos
                var cadenaAzure = builder.Configuration["ConnectionStrings-DefaultConnection"];

                // Configuración del contexto de EF
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(cadenaAzure)
                        .AddInterceptors(new CustomDbCommandInterceptor())
                        .AddInterceptors(new PerformanceInterceptor()));


                #endregion


                // Registro de repositorios y servicios
                builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));


                #region Manejadores CQRS    MediatR parte 1


                builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                    Assembly.GetExecutingAssembly(),
                    typeof(Empresa.Inv.Application.Entidades.ProductEntity.Handlers.GetProductByIdQueryHandler).Assembly
                ));

                #endregion


                builder.Services.AddScoped<IProductCustomRepository, ProductCustomRepository>();
                builder.Services.AddScoped<IInvAppService, InvAppService>();
                builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


                builder.Services.AddTransient<IEmailSender, EmailSender>();
                // Registrar TwoFactorSettings en el contenedor de servicios
                builder.Services.Configure<TwoFactorSettings>(builder.Configuration.GetSection("TwoFactorAuthentication"));


                // Configuración de Swagger
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JwtExample API", Version = "v1" });
                });


                #region MeditR parte 2
                // Configurar MediatR
                builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

                #endregion


                // Configuración de AutoMapper
                builder.Services.AddAutoMapper(typeof(MappingProfile));

                // Configuración del servicio de caché
                builder.Services.AddMemoryCache();
                builder.Services.AddSingleton<CacheService>();

                var app = builder.Build();


                // Registrar el middleware personalizado para captura de la respuesta
                app.UseMiddleware<ResponseLoggingMiddleware>();


                // Middleware de manejo de excepciones
                app.UseMiddleware<ExceptionHandlingMiddleware>();

                // Configuración de Swagger
                if (app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "local")
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JwtExample API v1");
                    });
                }

                // Utilización de routing
                app.UseRouting();

                // Implementación de política CORS
                app.UseCors("AllowSpecificOrigin");

                // Middleware de autenticación y autorización
                app.UseAuthentication();
                app.UseAuthorization();

                // Configuración de HTTPS
                app.UseHttpsRedirection();

                // Mapeo de controladores
                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "La aplicación falló al iniciar.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

     
    }
}
