using HMS.Application.Contracts.Services;
using HMS.Application.Models.Email;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace HMS.Application.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public SmtpEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(EmailMessageDto email)
        {
            var host = _config["EmailSettings:SmtpServer"];
            var port = int.Parse(_config["EmailSettings:Port"] ?? "587");
            var username = _config["EmailSettings:Username"];
            var password = _config["EmailSettings:Password"];
            var from = _config["EmailSettings:Sender"];
            var useSsl = bool.Parse(_config["EmailSettings:UseSsl"] ?? "true");

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = useSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(from!),
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email.To);

            await client.SendMailAsync(mailMessage);
        }
    }
}
