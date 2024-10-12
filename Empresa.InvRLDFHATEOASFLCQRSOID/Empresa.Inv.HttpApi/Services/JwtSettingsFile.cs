namespace Empresa.Inv.HttpApi.Services
{

    public class JwtSettingsFile
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int ExpiresInMinutes { get; set; }
        public string? PrivateKey { get; set; }
        public string? PublicKey { get; set; }

        public int RefreshTokenExpiresInDays { get; set; }

    }


}
