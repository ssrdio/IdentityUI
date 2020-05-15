using Microsoft.Extensions.Options;
using SSRD.IdentityUI.Account.Areas.Account.Models.Account;
using SSRD.IdentityUI.Core.Models.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Services.Account
{
    internal class AccountDataService : IAccountDataService
    {
        private readonly IdentityUIEndpoints _identityUIEndpoints;

        public AccountDataService(IOptions<IdentityUIEndpoints> identityUIEndpoints)
        {
            _identityUIEndpoints = identityUIEndpoints.Value;
        }

        public LoginViewModel GetLoginViewModel(string returnUrl)
        {
            LoginViewModel loginViewModel = new LoginViewModel(
                returnUrl: returnUrl,
                registrationEnabled: _identityUIEndpoints.RegisterEnabled,
                passwordRecoveryEnabled: _identityUIEndpoints.UseEmailSender ?? false);

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
    }
}
