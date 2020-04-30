using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Models.Result;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Interfaces.Services;

namespace SSRD.IdentityUI.Core.Services.Email
{
    internal class EmailConfirmationService : IEmailConfirmationService
    {
        private readonly UserManager<AppUserEntity> _userManager;

        private readonly ILogger<EmailConfirmationService> _logger;

        private readonly IEmailService _emailService;

        private readonly IdentityUIOptions _identityManagementOptions;
        private readonly IdentityUIEndpoints _identityManagementEndpoints;

        public EmailConfirmationService(UserManager<AppUserEntity> userManager, ILogger<EmailConfirmationService> logger, IEmailService emailService,
            IOptionsSnapshot<IdentityUIOptions> identityManagementOptions, IOptionsSnapshot<IdentityUIEndpoints> identityManagementEndpoints)
        {
            _userManager = userManager;

            _emailService = emailService;

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

            string token = HtmlEncoder.Default.Encode(callbackUrl);

            await _emailService.SendConfirmation(appUser.Email, token);

            return Result.Ok();
        }
    }
}
