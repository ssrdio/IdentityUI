using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using SSRD.IdentityUI.Account.Areas.Account.Models.Account;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Models.Result;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Services
{
    internal class AccountDataService : IAccountDataService
    {
        private readonly IdentityUIEndpoints _identityUIEndpoints;

        private readonly SignInManager<AppUserEntity> _signInManager;

        private readonly ILogger<AccountDataService> _logger;

        public AccountDataService(IOptions<IdentityUIEndpoints> identityUIEndpoints, SignInManager<AppUserEntity> signInManager,
            ILogger<AccountDataService> logger)
        {
            _identityUIEndpoints = identityUIEndpoints.Value;
            _signInManager = signInManager;

            _logger = logger;
        }

        public async Task<LoginViewModel> GetLoginViewModel(string returnUrl, string error = null)
        {
            LoginViewModel loginViewModel = new LoginViewModel(
                returnUrl: returnUrl,
                registrationEnabled: _identityUIEndpoints.RegisterEnabled,
                passwordRecoveryEnabled: _identityUIEndpoints.UseEmailSender ?? false,
                groupRegistrationEnabled: _identityUIEndpoints.GroupRegistrationEnabled,
                error: error,
                externalLogins: (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList());

            return loginViewModel;
        }

        public RegisterSuccessViewModel GetRegisterSuccessViewModel()
        {
            RegisterSuccessViewModel registerSuccessViewModel = new RegisterSuccessViewModel(
                useEmailSender: _identityUIEndpoints.UseEmailSender ?? false);

            return registerSuccessViewModel;
        }

        public RegisterViewModel GetRegisterViewModel()
        {
            RegisterViewModel registerViewModel = new RegisterViewModel(
                recoverPasswordEnabled: _identityUIEndpoints.UseEmailSender ?? false);

            return registerViewModel;
        }

        public async Task<Result<ExternalLoginRegisterViewModel>> GetExternalLoginViewModel(string returnUrl)
        {
            ExternalLoginInfo externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                _logger.LogError($"Error getting external login info");
                return Result.Fail<ExternalLoginRegisterViewModel>("failed_to_get_external_longin_info", "Failed to get external login info");
            }

            ExternalLoginRegisterViewModel externalLoginRegisterViewModel = new ExternalLoginRegisterViewModel(
                email: externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email),
                firstName: externalLoginInfo.Principal.FindFirstValue(ClaimTypes.GivenName),
                lastName: externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Surname),
                externalLoginProviderName: externalLoginInfo.LoginProvider,
                returnUrl: returnUrl);

            return Result.Ok(externalLoginRegisterViewModel);
        }

        public RegisterGroupViewModel GetRegisterGroupViewModel()
        {
            RegisterGroupViewModel registerGroupViewModel = new RegisterGroupViewModel(
                recoverPasswordEnabled: _identityUIEndpoints.UseEmailSender ?? false);

            return registerGroupViewModel;
        }
    }
}
