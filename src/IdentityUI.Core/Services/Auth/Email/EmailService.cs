using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Models.Result;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Auth.Email
{
    internal class EmailService : IEmailService
    {
        private readonly UserManager<AppUserEntity> _userManager;

        private readonly ILogger<EmailService> _logger;

        private readonly IEmailSender _emailSender;

        private readonly IdentityUIOptions _identityManagementOptions;
        private readonly IdentityUIEndpoints _identityManagementEndpoints;

        public EmailService(UserManager<AppUserEntity> userManager, ILogger<EmailService> logger, IEmailSender emailSender,
            IOptionsSnapshot<IdentityUIOptions> identityManagementOptions, IOptionsSnapshot<IdentityUIEndpoints> identityManagementEndpoints)
        {
            _userManager = userManager;

            _emailSender = emailSender;

            _logger = logger;

            _identityManagementOptions = identityManagementOptions.Value;
            _identityManagementEndpoints = identityManagementEndpoints.Value;
        }

        public async Task<Result> ConfirmEmail(string userId, string code)
        {
            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                _logger.LogError($"No user. User Id {userId}");
                return Result.Fail("error", "Error");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            IdentityResult result = await _userManager.ConfirmEmailAsync(appUser, code);
            if (!result.Succeeded)
            {
                _logger.LogError($"Error confirming email with code. UserId {userId}");
                return Result.Fail("error", "Error");
            }

            _logger.LogInformation($"Email confirmed. UserId {userId}");

            return Result.Ok();
        }

        public async Task<Result> SendVerificationMail(AppUserEntity appUser, string code)
        {
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            string callbackUrl = QueryHelpers.AddQueryString($"{_identityManagementOptions.BasePath}{_identityManagementEndpoints.ConfirmeEmail}/{appUser.Id}", "code", code);

            string mailBody = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";
            string subject = $"Confirm your email";

            await _emailSender.SendEmailAsync(appUser.Email, subject, mailBody);

            return Result.Ok();
        }
    }
}
