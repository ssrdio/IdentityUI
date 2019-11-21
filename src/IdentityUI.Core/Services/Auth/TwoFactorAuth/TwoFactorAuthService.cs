using FluentValidation;
using FluentValidation.Results;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth
{
    internal class TwoFactorAuthService : ITwoFactorAuthService
    {
        private readonly UserManager<AppUserEntity> _userManager;

        private readonly ILoginService _loginService;

        private readonly IValidator<AddTwoFactorAuthenticatorRequest> _addTwoFactorValidator;

        private readonly ILogger<TwoFactorAuthService> _logger;

        public TwoFactorAuthService(UserManager<AppUserEntity> userManager, IValidator<AddTwoFactorAuthenticatorRequest> addTwoFactorValidator,
            ILoginService loginService, ILogger<TwoFactorAuthService> logger)
        {
            _userManager = userManager;

            _loginService = loginService;

            _addTwoFactorValidator = addTwoFactorValidator;

            _logger = logger;
        }

        public async Task<Result> Disable(string userId)
        {
            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if(appUser == null)
            {
                _logger.LogWarning($"No user. UserId {userId}");
                return Result.Fail("no_user", "No User");
            }

            IdentityResult identityResult = await _userManager.SetTwoFactorEnabledAsync(appUser, false);
            if(!identityResult.Succeeded)
            {
                _logger.LogWarning($"Faild to disable 2fa. UserId {userId}");
                return Result.Fail(identityResult.Errors);
            }

            //TO DO: Maybe change user tokens

            _logger.LogInformation($"2fa disabled. UserId {userId}");

            return Result.Ok();
        }

        public async Task<Result<(string sharedKey, string authenticatorUri)>> Generate2faCode(string userId, string sessionCode, string ip)
        {
            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                _logger.LogError($"No user. UserId {userId}");
                return Result.Fail<(string sharedKey, string authenticatorUri)>($"no_user", $"No User");
            }

            string key = await _userManager.GetAuthenticatorKeyAsync(appUser);
            if (string.IsNullOrEmpty(key))
            {
                IdentityResult identityResult = await _userManager.ResetAuthenticatorKeyAsync(appUser);
                if(!identityResult.Succeeded)
                {
                    _logger.LogWarning($"Faild to reset authentication key. UserId {userId}");
                    return Result.Fail<(string, string)>(identityResult.Errors);
                }

                Result loginResult = await _loginService.Login(userId, sessionCode, ip);
                if (loginResult.Failure)
                {
                    _logger.LogError($"Faild to login user after 2fa reset. UserId {userId}");
                }

                key = await _userManager.GetAuthenticatorKeyAsync(appUser);
            }

            string sharedKey = FormatKey(key);
            string authenticatorUri = GenerateQrCodeUri(appUser.UserName, key);

            _logger.LogInformation($"2fa code generated. UserId {userId}");

            return Result.Ok((sharedKey: sharedKey, authenticatorUri: authenticatorUri));
        }

        public async Task<Result> Reset(string userId, string sessionCode, string ip)
        {
            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if(appUser == null)
            {
                _logger.LogWarning($"No user. UserId {userId}");
                return Result.Fail("no_user", "No user");
            }

            IdentityResult disable2faResult = await _userManager.SetTwoFactorEnabledAsync(appUser, false);
            if (!disable2faResult.Succeeded)
            {
                _logger.LogWarning($"Faild to disable 2fa. UserId {userId}");
                return Result.Fail(disable2faResult.Errors);
            }

            IdentityResult resetKeyResult = await _userManager.ResetAuthenticatorKeyAsync(appUser);
            if(!resetKeyResult.Succeeded)
            {
                _logger.LogWarning($"Faild to reset authentication key. UserId {userId}");
                return Result.Fail(resetKeyResult.Errors);
            }

            _logger.LogInformation($"Authentication key reset. UserId {userId}");

            Result loginResult = await _loginService.Login(userId, sessionCode, ip);
            if(loginResult.Failure)
            {
                _logger.LogError($"Faild to login user after 2fa reset. UserId {userId}");
            }

            return Result.Ok();
        }

        public async Task<Result> VerifyTwoFactorCode(string userId, string sessionCode, string ip, AddTwoFactorAuthenticatorRequest request)
        {
            ValidationResult validationResult = _addTwoFactorValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogError($"Invalid TwoFactor Verificatin code. ");
                return Result.Fail(ResultUtils.ToResultError(validationResult.Errors));
            }

            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                _logger.LogError($"No user. UserId {userId}");
                return Result.Fail("no_user", "No User");
            }

            string vereficationCode = request.VereficationCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            bool isCodeValid = await _userManager.VerifyTwoFactorTokenAsync(appUser, _userManager.Options.Tokens.AuthenticatorTokenProvider, vereficationCode);
            if (!isCodeValid)
            {
                _logger.LogError($"Invlid TwoFactor Verification code. User {userId}");
                return Result.Fail("invlid_code", "Invalid Code", "VereficationCode");
            }

            await _userManager.SetTwoFactorEnabledAsync(appUser, true);
            _logger.LogInformation($"2fa enabled. User {appUser.Id}");

            //TO DO: generate recovery codes

            Result loginResult = await _loginService.Login(userId, sessionCode, ip);
            if (loginResult.Failure)
            {
                _logger.LogError($"Faild to login user after 2fa reset. UserId {userId}");
            }

            return Result.Ok();
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6", //TO DO: move to appsettings
                HtmlEncoder.Default.Encode("test"), //TO DO: move to appseting
                HtmlEncoder.Default.Encode(email),
                unformattedKey);
        }
    }
}
