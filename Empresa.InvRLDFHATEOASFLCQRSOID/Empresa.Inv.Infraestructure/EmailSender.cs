
using Empresa.Inv.Dtos;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Text;
using System.Threading.Tasks;

namespace Empresa.Inv.Infraestructure
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;

            email.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, _emailSettings.UseSsl);

                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
        }
    }


}
