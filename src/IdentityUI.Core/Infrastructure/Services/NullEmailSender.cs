using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Infrastructure.Services
{
    internal class NullEmailSender : IEmailSender
    {
        private readonly ILogger<NullEmailSender> _logger;

        public NullEmailSender(ILogger<NullEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation($"NullEmailSender logging mail");
            _logger.LogDebug($"Send email. To {email}, {htmlMessage}");

            return Task.CompletedTask;
        }
    }
}
