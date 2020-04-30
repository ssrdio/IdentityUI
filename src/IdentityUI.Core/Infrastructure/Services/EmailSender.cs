using SSRD.IdentityUI.Core.Models.Options;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SSRD.IdentityUI.Core.Infrastructure.Services
{
    internal class EmailSender : IEmailSender
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _senderName;

        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptionsSnapshot<EmailSenderOptions> options, ILogger<EmailSender> logger)
        {
            _logger = logger;

            if (options.Value == null)
            {
                _logger.LogError($"EmailSender options are null");
                throw new Exception($"EmailSender options are null");
            }

            EmailSenderOptions emailSender = options.Value;

            bool enambleSsl = true;

#if DEBUG
            enambleSsl = false;
#endif

            _smtpClient = new SmtpClient(emailSender.Ip, emailSender.Port)
            {
                DeliveryFormat = SmtpDeliveryFormat.International,
                EnableSsl = enambleSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailSender.UserName, emailSender.Password)
            };

            _senderName = emailSender.SenderName;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailMessage mailMessage = new MailMessage(
                from: _senderName,
                to: email,
                subject: subject,
                body: htmlMessage)
            {
                IsBodyHtml = true,
            };

            try
            {
                _smtpClient.Send(mailMessage);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error during sending email. {ex}");
            }

            return Task.CompletedTask;
        }
    }
}
