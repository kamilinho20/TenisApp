using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using TenisApp.Core.Model;

namespace TenisApp.Api.Service
{
    public class EmailSender : IEmailSender
    {
        private readonly MailerConfig _config;

        public EmailSender(MailerConfig config)
        {
            _config = config;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_config.Server, _config.Port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_config.Login, _config.Password)
            };

            return client.SendMailAsync(new MailMessage(_config.Login, email, subject, htmlMessage)
                {IsBodyHtml = true});
        }
    }
}