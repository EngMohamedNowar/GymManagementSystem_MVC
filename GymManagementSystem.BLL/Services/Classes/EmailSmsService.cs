using GymManagementSystem.BLL.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class EmailSmsService : IEmailSmsService
    {
        private readonly ILogger<EmailSmsService> _logger;
        private readonly IConfiguration _configuration;

        public EmailSmsService(ILogger<EmailSmsService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task SendEmailAsync(string to, string subject, string body, CancellationToken ct = default)
        {
            var host = _configuration["Smtp:Host"];
            var portStr = _configuration["Smtp:Port"];
            var user = _configuration["Smtp:User"];
            var pass = _configuration["Smtp:Pass"];

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass)
                || !int.TryParse(portStr, out var port))
            {
                _logger.LogInformation("SMTP not configured. Skipping email to {To}: {Subject}", to, subject);
                return Task.CompletedTask;
            }

            try
            {
                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(user, pass),
                    EnableSsl = true
                };

                var from = _configuration["Smtp:From"] ?? user;
                var mail = new MailMessage(from, to, subject, body) { IsBodyHtml = false };
                return client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send email to {To}", to);
                return Task.CompletedTask;
            }
        }

        public Task SendSmsAsync(string to, string message, CancellationToken ct = default)
        {
            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            var fromNumber = _configuration["Twilio:FromNumber"];

            if (string.IsNullOrWhiteSpace(accountSid) || string.IsNullOrWhiteSpace(authToken) || string.IsNullOrWhiteSpace(fromNumber))
            {
                _logger.LogInformation("Twilio not configured. Skipping SMS to {To}: {Message}", to, message);
                return Task.CompletedTask;
            }

            _logger.LogInformation("Twilio configured; SMS dispatch to {To} is a stub. Message: {Message}", to, message);
            return Task.CompletedTask;
        }
    }
}
