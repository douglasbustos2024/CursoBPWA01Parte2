using Microsoft.ApplicationInsights;

namespace Empresa.Inv.HttpApi.Services
{
    public class ResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TelemetryClient _telemetryClient;

        public ResponseLoggingMiddleware(RequestDelegate next, TelemetryClient telemetryClient)
        {
            _next = next;
            _telemetryClient = telemetryClient;
        }

        public async Task Invoke(HttpContext context)
        {
            // Guardar el stream original de la respuesta
            var originalBodyStream = context.Response.Body;

            // Crear un MemoryStream para capturar la respuesta
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                // Procesar la solicitud
                await _next(context);

                // Leer el cuerpo de la respuesta
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                // Enviar los datos de la respuesta a Application Insights
                _telemetryClient.TrackTrace("Response Data", new Dictionary<string, string>
            {
                { "StatusCode", context.Response.StatusCode.ToString() },
                { "ResponseBody", responseBodyText }
            });

                // Volver a escribir el cuerpo de la respuesta original
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }

}
