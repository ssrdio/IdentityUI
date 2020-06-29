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
using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using SSRD.IdentityUI.Core.Models.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;

namespace SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth
{
    internal class TwoFactorAuthService : ITwoFactorAuthService
    {
        /// <summary>
        /// 0 = userid
        /// </summary>
        private const string SEND_EMAIL_KEY = "send_2fa_emil_{0}";
        private const string SEND_SMS_KEY = "send_2fa_sms_{0}";

        private const int SEND_TIMEOUT_SECONDS = 5;

        private readonly UserManager<AppUserEntity> _userManager;
        private readonly SignInManager<AppUserEntity> _signInManager;

        private readonly ILoginService _loginService;

        private readonly IEmailService _emailService;
        private readonly ISmsSender _smsSender;
        private readonly IMemoryCache _memoryCache;

        private readonly IValidator<AddTwoFactorAuthenticatorRequest> _addTwoFactorValidator;
        private readonly IValidator<AddTwoFactorPhoneAuthenticationRequest> _addPhoneTwoFactorValidator;
        private readonly IValidator<AddTwoFactorEmailAuthenticationRequest> _addEmailTwoFactorValidator;

        private readonly IdentityUIEndpoints _identityUIEndpoints;

        private readonly ILogger<TwoFactorAuthService> _logger;

        public TwoFactorAuthService(
            UserManager<AppUserEntity> userManager,
            SignInManager<AppUserEntity> signInManager,
            IValidator<AddTwoFactorAuthenticatorRequest> addTwoFactorValidator, 
            IValidator<AddTwoFactorPhoneAuthenticationRequest> addPhoneTwoFactorValidator,
            IValidator<AddTwoFactorEmailAuthenticationRequest> addEmailTwoFactorValidator,
            ILoginService loginService,
            IEmailService emailService,
            ISmsSender smsSender,
            IMemoryCache memoryCache,
            IOptions<IdentityUIEndpoints> identityUIEndpoints,
            ILogger<TwoFactorAuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;

            _loginService = loginService;

            _emailService = emailService;
            _smsSender = smsSender;
            _memoryCache = memoryCache;

            _addTwoFactorValidator = addTwoFactorValidator;
            _addPhoneTwoFactorValidator = addPhoneTwoFactorValidator;
            _addEmailTwoFactorValidator = addEmailTwoFactorValidator;

            _identityUIEndpoints = identityUIEndpoints.Value;

            _logger = logger;
        }

        public async Task<Result<(string sharedKey, string authenticatorUri)>> Generate2faCode(string userId)
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
                if (!identityResult.Succeeded)
                {
                    _logger.LogWarning($"Failed to reset authentication key. UserId {userId}");
                    return Result.Fail<(string, string)>(identityResult.Errors);
                }

                Result loginResult = await _loginService.Login(userId);
                if (loginResult.Failure)
                {
                    _logger.LogError($"Failed to login user after 2fa reset. UserId {userId}");
                }

                key = await _userManager.GetAuthenticatorKeyAsync(appUser);
            }

            string sharedKey = FormatKey(key);
            string authenticatorUri = GenerateQrCodeUri(appUser.UserName, key);

            _logger.LogInformation($"2fa code generated. UserId {userId}");

            return Result.Ok((sharedKey: sharedKey, authenticatorUri: authenticatorUri));
        }

        public async Task<Result> GenerateSmsCode(string userId)
        {
            string timeoutKey = string.Format(SEND_SMS_KEY, userId);

            bool timeout = _memoryCache.TryGetValue(timeoutKey, out string temp);
            if (timeout)
            {
                return Result.Fail("to_many_requests", "To many requests");
            }

            _memoryCache.Set(timeoutKey, "temp", TimeSpan.FromSeconds(SEND_TIMEOUT_SECONDS));

            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                _logger.LogError($"No user. UserId {userId}");
                return Result.Fail($"no_user", "No User");
            }

            if (string.IsNullOrEmpty(appUser.PhoneNumber))
            {
                _logger.LogError($"User does not have a phone number. UserId {userId}");
                return Result.Fail($"phone_number_not_found", "Phone number not found");
            }

            string code = await _userManager.GenerateTwoFactorTokenAsync(appUser, TwoFactorAuthenticationType.Phone.ToProvider());

            Result smsSendResult = await _smsSender.Send(appUser.PhoneNumber, $"Authentication code: {code}");
            if (smsSendResult.Failure)
            {
                return Result.Fail(smsSendResult.Errors);
            }

            return Result.Ok();
        }

        public async Task<Result> GenerateAndSendEmailCode(string userId)
        {
            string timeoutKey = string.Format(SEND_EMAIL_KEY, userId);

            bool timeout = _memoryCache.TryGetValue(timeoutKey, out string temp);
            if(timeout)
            {
                return Result.Fail("to_many_requests", "To many requests");
            }

            _memoryCache.Set(timeoutKey, "temp", TimeSpan.FromSeconds(SEND_TIMEOUT_SECONDS));

            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                _logger.LogError($"No user. UserId {userId}");
                return Result.Fail<string>($"no_user", "No User");
            }

            if(string.IsNullOrEmpty(appUser.Email))
            {
                _logger.LogError($"User does not have an email address");
                return Result.Fail<string>("no_email", "No email");
            }

            string code = await _userManager.GenerateTwoFactorTokenAsync(appUser, TwoFactorAuthenticationType.Email.ToProvider());

            Result sendTokenResult = await _emailService.Send2faToken(appUser.Email, code);
            if(sendTokenResult.Failure)
            {
                return Result.Fail(sendTokenResult.Errors);
            }

            return Result.Ok();
        }

        public async Task<Result> Disable(string userId)
        {
            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if(appUser == null)
            {
                _logger.LogWarning($"No user. UserId {userId}");
                return Result.Fail("no_user", "No user");
            }

            appUser.TwoFactor = TwoFactorAuthenticationType.None;

            IdentityResult disable2faResult = await _userManager.SetTwoFactorEnabledAsync(appUser, false);
            if (!disable2faResult.Succeeded)
            {
                _logger.LogWarning($"Failed to disable 2fa. UserId {userId}");
                return Result.Fail(disable2faResult.Errors);
            }

            IdentityResult resetKeyResult = await _userManager.ResetAuthenticatorKeyAsync(appUser);
            if(!resetKeyResult.Succeeded)
            {
                _logger.LogWarning($"Failed to reset authentication key. UserId {userId}");
                return Result.Fail(resetKeyResult.Errors);
            }

            _logger.LogInformation($"Authentication key reset. UserId {userId}");

            Result loginResult = await _loginService.Login(userId);
            if(loginResult.Failure)
            {
                _logger.LogError($"Failed to login user after 2fa reset. UserId {userId}");
            }

            return Result.Ok();
        }

        public Task<Result<IEnumerable<string>>> VerifyTwoFactorCode(string userId, AddTwoFactorAuthenticatorRequest request)
        {
            ValidationResult validationResult = _addTwoFactorValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogError($"Invalid TwoFactor Verification code. ");
                return Task.FromResult(Result.Fail<IEnumerable<string>>(ResultUtils.ToResultError(validationResult.Errors)));
            }

            string vereficationCode = request.VereficationCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            return AddTwoFactorAuthentication(userId, TwoFactorAuthenticationType.Authenticator, vereficationCode);
        }

        public Task<Result<IEnumerable<string>>> VerifyPhoneTwoFactorCode(string userId, AddTwoFactorPhoneAuthenticationRequest request)
        {
            ValidationResult validationResult = _addPhoneTwoFactorValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogError($"Verification failed.");
                return Task.FromResult(Result.Fail<IEnumerable<string>>(ResultUtils.ToResultError(validationResult.Errors)));
            }

            return AddTwoFactorAuthentication(userId, TwoFactorAuthenticationType.Email, request.Token);
        }

        public Task<Result<IEnumerable<string>>> VerifyEmailTwoFactorCode(string userId, AddTwoFactorEmailAuthenticationRequest request)
        {
            ValidationResult validationResult = _addEmailTwoFactorValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogError($"Verification failed.");
                return Task.FromResult(Result.Fail<IEnumerable<string>>(ResultUtils.ToResultError(validationResult.Errors)));
            }

            return AddTwoFactorAuthentication(userId, TwoFactorAuthenticationType.Email, request.Token);
        }

        private async Task<Result<IEnumerable<string>>> AddTwoFactorAuthentication(string userId, TwoFactorAuthenticationType twoFactorAuthenticationType, string token)
        {
            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                _logger.LogError($"No user. UserId {userId}");
                return Result.Fail<IEnumerable<string>>("no_user", "No User");
            }

            bool isCodeValid = await _userManager.VerifyTwoFactorTokenAsync(appUser, twoFactorAuthenticationType.ToProvider(), token);
            if (!isCodeValid)
            {
                _logger.LogError($"Invalid TwoFactor Verification code. User {userId}");
                return Result.Fail<IEnumerable<string>>("invlid_code", "Invalid Code");
            }

            await _userManager.SetTwoFactorEnabledAsync(appUser, true);

            appUser.TwoFactor = twoFactorAuthenticationType;
            if(twoFactorAuthenticationType == TwoFactorAuthenticationType.Phone)
            {
                appUser.PhoneNumberConfirmed = true; //HACK: try to do this before you add 2fa
            }

            await _userManager.UpdateAsync(appUser);

            _logger.LogInformation($"TwoFactorAuthentication enabled. User {appUser.Id}");

            Result loginResult = await _loginService.Login(userId);
            if (loginResult.Failure)
            {
                _logger.LogError($"Failed to login user after enabling 2fa. UserId {userId}");
            }

            IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(appUser, _identityUIEndpoints.NumberOfRecoveryCodes);

            return Result.Ok(recoveryCodes);
        }

        public async Task<Result> TrySend2faCode(string userId)
        {
            AppUserEntity appUser = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (appUser == null)
            {
                _logger.LogError($"No user for Twofactor login");
                 return Result.Fail("no_user", "No User");
            }

            if (!appUser.TwoFactorEnabled)
            {
                _logger.LogError($"2fa for user is disabled. UserId {userId}");
                return Result.Fail("invalid_action", "Invalid action");
            }

            if (appUser.TwoFactor == TwoFactorAuthenticationType.None)
            {
                _logger.LogError($"2fa enabled, but type is not set. UserId {userId}");
                return Result.Fail("invalid_action", "Invalid action");
            }

            if (appUser.TwoFactor == TwoFactorAuthenticationType.Phone)
            {
                Result createCodeResult = await GenerateSmsCode(appUser.Id);
                if (createCodeResult.Failure)
                {
                    return Result.Fail(createCodeResult.Errors);
                }
                _logger.LogInformation($"2fa code sent. UserId {userId}");
            }

            if (appUser.TwoFactor == TwoFactorAuthenticationType.Email)
            {
                Result createCodeResult = await GenerateAndSendEmailCode(appUser.Id);
                if (createCodeResult.Failure)
                {
                    return Result.Fail(createCodeResult.Errors);
                }
                _logger.LogInformation($"2fa code sent. UserId {userId}");
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

        private string GenerateQrCodeUri(string username, string unformattedKey)
        {
            return string.Format(
                "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
                HtmlEncoder.Default.Encode(_identityUIEndpoints.AuthenticatorIssuer),
                HtmlEncoder.Default.Encode(username),
                unformattedKey);
        }
    }
}
