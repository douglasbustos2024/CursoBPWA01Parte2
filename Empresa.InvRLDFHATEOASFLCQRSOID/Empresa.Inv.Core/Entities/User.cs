namespace Empresa.Inv.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public DateTime Created { get; set; }

        public string Roles { get; set; }

        public string? TwoFactorCode { get; set; }
        public DateTime? TwoFactorExpiry { get; set; }



    }
}
