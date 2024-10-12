namespace Empresa.Inv.Dtos
{
    public static class ResponseApiService
    {
        public static BaseResponseModel Response(int statusCode, object? Data = null, string? message = null)
        {
            bool succes = false;

            if (statusCode >= 200 && statusCode <= 300)
                succes = true;

            var result = new BaseResponseModel
            {
                StatusCode = statusCode,
                Success = succes,
                Message = message ?? string.Empty, // Asignar valor predeterminado
                Data = Data ?? new object()        // Asignar objeto vacío en lugar de nul
            };

            return result;
        }

    }
}
