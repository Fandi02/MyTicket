using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace MyTicket.WebApi.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Mengambil konfigurasi SMTP dari appsettings.json
            var smtpHost = _configuration["SmtpOptions:Hostname"];
            var smtpPort = Convert.ToInt32(_configuration["SmtpOptions:Port"]);
            var smtpUser = _configuration["SmtpOptions:Username"];
            var smtpPass = _configuration["SmtpOptions:Password"];

            using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPass);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser),  // menggunakan email dari konfigurasi
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                // Mengirim email secara asinkron
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}