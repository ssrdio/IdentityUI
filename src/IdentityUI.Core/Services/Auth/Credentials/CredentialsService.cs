using FluentValidation;
using FluentValidation.Results;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.Credentials.Models;
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
using SSRD.IdentityUI.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using SSRD.IdentityUI.Core.Services.Identity;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SSRD.IdentityUI.Core.Services.Auth.Credentials
{
    internal class CredentialsService : ICredentialsService
    {
        private readonly UserManager<AppUserEntity> _userManager;

        private readonly IEmailService _emailService;
        private readonly ILoginService _loginService;

        private readonly IIdentityUIUserInfoService _identityUIUserInfoService;

        private readonly IValidator<RecoverPasswordRequest> _forgotPasswordValidator;
        private readonly IValidator<ResetPasswordRequest> _recoverPasswordValidator;
        private readonly IValidator<ChangePasswordRequest> _changePasswordValidator;
        private readonly IValidator<CreatePasswordRequest> _createPasswordValidator;

        private readonly ILogger<CredentialsService> _logger;

        private readonly IdentityUIOptions _identityManagementOptions;
        private readonly IdentityUIEndpoints _identityManagementEndpoints;

        public CredentialsService(
            UserManager<AppUserEntity> userManager,
            IEmailService emailService,
            ILoginService loginService,
            IIdentityUIUserInfoService identityUIUserInfoService,
            IValidator<RecoverPasswordRequest> forgotPasswordValidator,
            IValidator<ResetPasswordRequest> recoverPasswordValidator,
            IValidator<ChangePasswordRequest> changePasswordValidator,
            IValidator<CreatePasswordRequest> createPasswordValidator,
            ILogger<CredentialsService> logger,
            IOptionsSnapshot<IdentityUIOptions> identityManagementOptions,
            IOptionsSnapshot<IdentityUIEndpoints> identityManagementEndpoints)
        {
            _userManager = userManager;

            _emailService = emailService;
            _loginService = loginService;

            _identityUIUserInfoService = identityUIUserInfoService;

            _forgotPasswordValidator = forgotPasswordValidator;
            _recoverPasswordValidator = recoverPasswordValidator;
            _changePasswordValidator = changePasswordValidator;
            _createPasswordValidator = createPasswordValidator;

            _logger = logger;

            _identityManagementOptions = identityManagementOptions.Value;
            _identityManagementEndpoints = identityManagementEndpoints.Value;
        }

        public async Task<Result> ChangePassword(string userId, string sessionCode, string ip, ChangePasswordRequest request)
        {
            ValidationResult validationResult = _changePasswordValidator.Validate(request);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid ChangePasswordrequest. UserId {userId}");
                return Result.Fail(validationResult.Errors);
            }

            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if(appUser == null)
            {
                _logger.LogError($"No user. UserId {userId}");
                return Result.Fail("no_user", "No user");
            }

            IdentityResult identityResult = await _userManager.ChangePasswordAsync(appUser, request.OldPassword, request.NewPassword);
            if(!identityResult.Succeeded)
            {
                _logger.LogWarning($"Failed to change password. UserId {userId}");
                return Result.Fail(identityResult.Errors);
            }

            _logger.LogInformation($"Changed password. UserId {userId}");

            Result loginResult = await _loginService.Login(userId, sessionCode, ip);
            if(loginResult.Failure)
            {
                _logger.LogError($"Failed to log in user after password change. UserId {userId}");
            }

            return Result.Ok();
        }

        public async Task<Result> RecoverPassword(RecoverPasswordRequest request)
        {
            ValidationResult validationResult = _forgotPasswordValidator.Validate(request);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"No user. User Email {request.Email}");
                return Result.Fail(validationResult.Errors);
            }

            AppUserEntity appUser = await _userManager.FindByEmailAsync(request.Email);
            if(appUser == null)
            {
                _logger.LogWarning($"No user. User email {request.Email}");
                return Result.Ok();
            }

            bool emailConfirmed = await _userManager.IsEmailConfirmedAsync(appUser);
            if(!emailConfirmed || !appUser.Enabled || appUser.LockoutEnd != null)
            {
                _logger.LogWarning($"User email not confirmed. UserId {appUser.Id}");
                return Result.Ok();
            }

            string code = await _userManager.GeneratePasswordResetTokenAsync(appUser);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            string calbackUrl = QueryHelpers.AddQueryString($"{_identityManagementOptions.BasePath}{_identityManagementEndpoints.ResetPassword}", "code", code);

            Result sendMail = await _emailService.SendPasswordRecovery(appUser.Email, calbackUrl);
            if(sendMail.Failure)
            {
                return Result.Fail(sendMail.Errors);
            }

            _logger.LogInformation($"Password recovery email send. UserId {appUser.Id}");

            return Result.Ok();
        }

        public async Task<Result> ResetPassword(ResetPasswordRequest request)
        {
            ValidationResult validationResult = _recoverPasswordValidator.Validate(request);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invlid RecoverPassword request");
                return Result.Fail(validationResult.Errors);
            }

            AppUserEntity appUser = await _userManager.FindByEmailAsync(request.Email);
            if(appUser == null)
            {
                _logger.LogWarning($"No user. Email {request.Email}");
                return Result.Ok();
            }

            bool emailConfirmed = await _userManager.IsEmailConfirmedAsync(appUser);
            if (!emailConfirmed || !appUser.Enabled || appUser.LockoutEnd != null)
            {
                _logger.LogWarning($"User email not confirmed. UserId {appUser.Id}");
                return Result.Ok();
            }

            string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            IdentityResult identityResult = await _userManager.ResetPasswordAsync(appUser, code, request.Password);
            if(!identityResult.Succeeded)
            {
                _logger.LogWarning($"Failed to reset password for user. User {appUser.Id}");
                return Result.Fail(identityResult.Errors);
            }

            Result sendMail = await _emailService.SendPasswordWasReset(appUser.Email);
            if (sendMail.Failure)
            {
                //return Result.Fail(sendMail.Errors);
            }

            _logger.LogInformation($"Password reset. UserId {appUser.Id}");

            return Result.Ok();
        }

        public async Task<Result> CreatePassword(CreatePasswordRequest createPasswordRequest)
        {
            ValidationResult validationResult = _createPasswordValidator.Validate(createPasswordRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(CreatePasswordRequest)} model");
                return Result.Fail(validationResult.Errors);
            }

            string userId = _identityUIUserInfoService.GetUserId();

            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if(appUser == null)
            {
                _logger.LogError($"No User. UserId {userId}");
                return Result.Fail("no_user", "No User");
            }

            _logger.LogTrace($"Creating password. UserId {userId}");

            IdentityResult identityResult = await _userManager.AddPasswordAsync(appUser, createPasswordRequest.NewPassword);
            if(!identityResult.Succeeded)
            {
                _logger.LogError($"Failed to add password. UserId {userId}");
                return Result.Fail(identityResult.Errors);
            }

            Result loginResult = await _loginService.Login(userId);
            if (loginResult.Failure)
            {
                _logger.LogError($"Failed to log in user after password was added. UserId {userId}");
            }

            return Result.Ok();
        }

        public async Task<Result> RemoveExternalLogin()
        {
            string userId = _identityUIUserInfoService.GetUserId();

            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                _logger.LogError($"No User. UserId {userId}");
                return Result.Fail("no_user", "No User");
            }

            _logger.LogTrace($"Removing external login provider. UserId {userId}");

            IList<UserLoginInfo> loginProviders = await _userManager.GetLoginsAsync(appUser);

            UserLoginInfo userLoginInfo = loginProviders.SingleOrDefault(); //TODO: only support one login provider
            if(userLoginInfo == null)
            {
                _logger.LogWarning($"User does not have a external login provider. UserId {userId}");
                return Result.Ok();
            }

            IdentityResult identityResult = await _userManager.RemoveLoginAsync(appUser, userLoginInfo.LoginProvider, userLoginInfo.ProviderKey);
            if(!identityResult.Succeeded)
            {
                _logger.LogError($"Failed to remove external login provider. UserId {userId}");
                return Result.Fail("failed_to_remove_external_login_provider", "Failed to remove external login provider");
            }

            Result loginResult = await _loginService.Login(userId);
            if (loginResult.Failure)
            {
                _logger.LogError($"Failed to log in user after external login provider was removed. UserId {userId}");
            }

            return Result.Ok();
        }
    }
}
