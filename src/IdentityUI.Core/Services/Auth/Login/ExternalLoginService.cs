using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using SSRD.IdentityUI.Core.Interfaces;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.Login.Models;
using SSRD.IdentityUI.Core.Services.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Auth.Login
{
    internal class ExternalLoginService : IExternalLoginService
    {
        private readonly SignInManager<AppUserEntity> _signInManager;
        private readonly UserManager<AppUserEntity> _userManager;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlGenerator _urlGenerator;
        private readonly ISessionService _sessionService;

        private readonly IdentityUIEndpoints _identityOptions;

        private readonly IValidator<ExternalLoginRequest> _externalLoginRequestValidator;

        private readonly ILogger<ExternalLoginService> _logger;

        public ExternalLoginService(SignInManager<AppUserEntity> signInManager, UserManager<AppUserEntity> userManager,
            IHttpContextAccessor httpContextAccessor, IUrlGenerator urlGenerator, ISessionService sessionService,
            IOptions<IdentityUIEndpoints> identityOptions, IValidator<ExternalLoginRequest> externalLoginRequestValidator,
            ILogger<ExternalLoginService> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;

            _httpContextAccessor = httpContextAccessor;
            _urlGenerator = urlGenerator;
            _sessionService = sessionService;

            _identityOptions = identityOptions.Value;

            _externalLoginRequestValidator = externalLoginRequestValidator;

            _logger = logger;
        }

        public async Task<Result<AuthenticationProperties>> ExternalLogin(ExternalLoginRequest externalLoginRequest, string returnUrl)
        {
            ValidationResult validationResult = _externalLoginRequestValidator.Validate(externalLoginRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(ExternalLoginRequest)} model");
                return Result.Fail<AuthenticationProperties>(validationResult.Errors);
            }

            await _signInManager.SignOutAsync();

            string redirectUrl = _urlGenerator.GenerateActionUrl("ExternalLoginCallback", "Account", new { returnUrl });

            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(externalLoginRequest.Provider, redirectUrl);

            return Result.Ok(properties);
        }

        public async Task<Result<SignInResult>> Callback(string remoteError)
        {
            if(!string.IsNullOrEmpty(remoteError))
            {
                _logger.LogError($"External login provider returned error. Error {remoteError}");
                return Result.Fail<SignInResult>("external_login_provider_error", remoteError);
            }

            ExternalLoginInfo externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if(externalLoginInfo == null)
            {
                _logger.LogError($"Error getting external login info");
                return Result.Fail<SignInResult>("failed_to_get_external_longin_info", "Failed to get external login info");
            }

            AppUserEntity appUser = await _userManager.FindByLoginAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);
            if(appUser == null)
            {
                _logger.LogInformation($"Users email does not exist");
                return Result.Ok(SignInResult.Failed);
            }

            string sessionCode = _httpContextAccessor.HttpContext.User.GetSessionCode();
            if (sessionCode != null)
            {
                _sessionService.Logout(sessionCode, appUser.Id, SessionEndTypes.Expired);
            }

            if (!appUser.CanLogin())
            {
                _logger.LogInformation($"User is not allowed to login. User {appUser.Id}");
                return Result.Fail<SignInResult>("no_user", "No user");
            }

            SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(
                loginProvider: externalLoginInfo.LoginProvider,
                providerKey: externalLoginInfo.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: _identityOptions.BypassTwoFactorOnExternalLogin);

            return Result.Ok(signInResult);
        }
    }
}
