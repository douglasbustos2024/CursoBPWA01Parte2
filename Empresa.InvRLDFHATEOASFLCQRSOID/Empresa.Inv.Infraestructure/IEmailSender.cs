using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empresa.Inv.Infraestructure
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
