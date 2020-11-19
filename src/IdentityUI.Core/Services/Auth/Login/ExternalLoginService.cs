using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.Login.Models;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Auth.Login
{
    internal class ExternalLoginService : IExternalLoginService
    {
        private readonly SignInManager<AppUserEntity> _signInManager;
        private readonly UserManager<AppUserEntity> _userManager;

        private readonly IIdentityUIUserInfoService _identityUIUserInfoService;
        private readonly ISessionService _sessionService;
        private readonly ILoginFilter _canLoginService;

        private readonly IdentityUIOptions _identityUIOptions;
        private readonly IdentityUIEndpoints _identityUIEndpoints;

        private readonly IValidator<ExternalLoginRequest> _externalLoginRequestValidator;

        private readonly ILogger<ExternalLoginService> _logger;

        public ExternalLoginService(
            SignInManager<AppUserEntity> signInManager,
            UserManager<AppUserEntity> userManager,
            IIdentityUIUserInfoService identityUIUserInfoService,
            ISessionService sessionService,
            ILoginFilter canLoginService,
            IOptions<IdentityUIOptions> identityUIOptions,
            IOptions<IdentityUIEndpoints> identityUIEndpoints,
            IValidator<ExternalLoginRequest> externalLoginRequestValidator,
            ILogger<ExternalLoginService> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;

            _identityUIUserInfoService = identityUIUserInfoService;
            _sessionService = sessionService;
            _canLoginService = canLoginService;

            _identityUIOptions = identityUIOptions.Value;
            _identityUIEndpoints = identityUIEndpoints.Value;

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

            string callbackUrl = QueryHelpers.AddQueryString($"{_identityUIOptions.BasePath}/Account/ExternalLoginCallback", "returnUrl", returnUrl);
            string redirectUrl = HtmlEncoder.Default.Encode(callbackUrl);

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

            string sessionCode = _identityUIUserInfoService.GetSessionCode();
            if (sessionCode != null)
            {
                _sessionService.Logout(sessionCode, appUser.Id, SessionEndTypes.Expired);
            }

            CommonUtils.Result.Result beforeLoginFilterResult = await _canLoginService.BeforeAdd(appUser);
            if (beforeLoginFilterResult.Failure)
            {
                _logger.LogInformation($"User is not allowed to login. User {appUser.Id}");
                beforeLoginFilterResult.ToOldResult();
            }

            SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(
                loginProvider: externalLoginInfo.LoginProvider,
                providerKey: externalLoginInfo.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: _identityUIEndpoints.BypassTwoFactorOnExternalLogin);

            CommonUtils.Result.Result afterLoginFilterResult = await _canLoginService.AfterAdded(appUser);
            if (afterLoginFilterResult.Failure)
            {
                await _signInManager.SignOutAsync();
                _sessionService.Logout(appUser.SessionCode, appUser.Id, SessionEndTypes.AffterLoginFilterFailure);

                _logger.LogInformation($"User is not allowed to login. User {appUser.Id}");
                afterLoginFilterResult.ToOldResult();
            }

            return Result.Ok(signInResult);
        }
    }
}
