

using Microsoft.AspNetCore.Identity;

namespace Empresa.Inv.HttpApi
{
    public class ApplicationUser : IdentityUser
    {
        public string TwoFactorCode { get; set; }
        public DateTime TwoFactorExpiry { get; set; }
    }
}
