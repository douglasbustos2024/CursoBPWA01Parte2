using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
 
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Empresa.Inv.Dtos;
using Empresa.Inv.HttpApi.Services;
using Empresa.Inv.Application.Shared.Entities.Dtos;
using Empresa.Inv.EntityFrameworkCore;
using Empresa.Inv.Core.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Empresa.Inv.HttpApi.Controllers
{
    public class TokenRequest
    {
        public string? Token { get; set; }
    }
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly JwtTokenService _jw;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;


        public AuthController(  JwtTokenService  jw, IRepository<User> userRepository,IMapper mapper)
        {
            _jw = jw;
            _userRepository = userRepository;
            _mapper= mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
                       

            var userDb =await _userRepository.Query().Where(u=>u.UserName==login.UserName && u.Password==login.Password ).FirstOrDefaultAsync();

            if (userDb == null)
                return Unauthorized();

            // Generar el Access Token y el Refresh Token
            var user = _mapper.Map<UserDto>(userDb);
            var tokenResponse =await _jw.GenerateToken(user, login.ClientType??string.Empty);

          

            var response = new AuthResponseDto
            {
                IsSuccess = true,
                AccessToken = tokenResponse.AccessToken,
             
            };



            // Retornar la respuesta con el token y el refresh token
            return Ok(response);

        }

        // Paso 2: Validar el Código 2FA
        [HttpPost("validate-2fa")]
        public async Task<IActionResult> ValidateTwoFactor([FromBody] TwoFactorDto twoFactorDto)
        {
            if (twoFactorDto == null || string.IsNullOrEmpty(twoFactorDto.Username) || string.IsNullOrEmpty(twoFactorDto.Code))
            {
                return BadRequest("Datos inválidos.");
            }

            // Obtener al usuario desde tu repositorio o servicio
            var user = await GetUserByUsername(twoFactorDto.Username);
            if (user == null)
            {
                return Unauthorized("Usuario no encontrado.");
            }

            try
            {
                var tokenResponse = await _jw.ValidateTwoFactorAndGenerateToken(user, twoFactorDto.Code);
                return Ok(tokenResponse);
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        private async Task<UserDto> GetUserByUsername(string username)
        {
            var ret = new UserDto();

           var userByUserName =await _userRepository.Query().Where(u => u.UserName == username).FirstOrDefaultAsync();

            if (userByUserName != null)
            {
                ret = new UserDto
                {
                    Id = 2,
                    UserName = userByUserName.UserName,
                    Email = userByUserName.Email,
                    Roles = userByUserName.Roles,
                    TwoFactorCode = userByUserName.TwoFactorCode,
                    TwoFactorExpiry = userByUserName.TwoFactorExpiry

                };
            }
             
             return ret;

      
        }


        [HttpPost("validate-token")]
        public  IActionResult  ValidateToken([FromBody] TokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest("Token is required.");
            }


            ObjectResult returned;

            try
            {
                // Validar el token
                var claimsPrincipal = _jw.ValidateToken(request.Token);

                var answer = false;

                if (claimsPrincipal != null)
                {
                    answer = true;
                }
                else
                    return Unauthorized(new { IsValid = false, Message = "Error al generar." });



                returned = StatusCode(StatusCodes.Status201Created, new { isSuccess = answer, Claims = claimsPrincipal.Claims.Select(c => new { c.Type, c.Value }) });


            }
            catch (SecurityTokenExpiredException)
            {
                return Unauthorized(new { IsValid = false, Message = "Token has expired." });
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(new { IsValid = false, Message = ex.Message });
            }

            return returned;

        }


        [HttpPost("logout")]                               
        public async Task<IActionResult> Logout()
        {                       

            ObjectResult returned;

            try
            {
                // Validar el token
                await HttpContext.SignOutAsync();
                                                                     

                returned = StatusCode(StatusCodes.Status200OK, new { isSuccess = true  });
                               
            }
            catch 
            {
                returned = StatusCode(StatusCodes.Status400BadRequest, new { isSuccess = true });
            }
                                 
            return returned;


        }

    }


}
