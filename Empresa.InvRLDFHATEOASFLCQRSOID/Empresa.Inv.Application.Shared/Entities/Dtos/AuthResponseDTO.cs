namespace Empresa.Inv.Dtos
{

    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpires { get; set; }
    }

    public class TokenResponseDto
    {
        public string? AccessToken { get; set; }
        public RefreshTokenDto RefreshToken { get; set; } = new RefreshTokenDto();
    }

    public class RefreshTokenDto
    {
        public string? Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
