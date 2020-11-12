using SSRD.IdentityUI.Core.Models.Options;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text;

#if NET_CORE3
using Microsoft.Extensions.Hosting;
#endif

namespace SSRD.IdentityUI.Core.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        protected readonly SmtpClient _smtpClient;
        protected readonly string _senderEmail;
        protected readonly string _senderDisplayName;

        protected readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptionsSnapshot<EmailSenderOptions> options, ILogger<EmailSender> logger)
        {
            _logger = logger;

            if (options.Value == null)
            {
                _logger.LogError($"EmailSender options are null");
                throw new ArgumentNullException($"EmailSender options are null");
            }

            EmailSenderOptions emailSender = options.Value;

            _smtpClient = new SmtpClient(emailSender.Ip, emailSender.Port)
            {
                DeliveryFormat = SmtpDeliveryFormat.International,
                Credentials = new NetworkCredential(emailSender.UserName, emailSender.Password),
                EnableSsl = emailSender.UseSSL,
            };

            _senderEmail = emailSender.SenderEmail;
            if(string.IsNullOrEmpty(_senderEmail))
            {
                _senderEmail = emailSender.SenderName;
            }

            _senderDisplayName = emailSender.SenderDisplayName;
        }

        public virtual Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailMessage mailMessage = new MailMessage(
                from: new MailAddress(_senderEmail, _senderDisplayName),
                to: new MailAddress(email))
            {
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                HeadersEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
            };

            try
            {
                _smtpClient.Send(mailMessage);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error during sending email. {ex}");
                throw ex;
            }

            return Task.CompletedTask;
        }
    }
}
