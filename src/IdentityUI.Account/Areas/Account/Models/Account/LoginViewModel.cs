using Microsoft.AspNetCore.Authentication;
using SSRD.AdminUI.Template.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Account
{
    public class LoginViewModel : IReCaptchaRequest
    {
        [DisplayName("Username")]
        public string UserName { get; set; }
        public string Password { get; set; }
        [DisplayName("Remember me")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
        public bool RegistrationEnabled { get; set; }
        public bool PasswordRecoveryEnabled { get; set; }
        public bool GroupRegistrationEnabled { get; set; }

        public string ReCaptchaToken { get; set; }

        public string Error { get; set; }
        public List<AuthenticationScheme> ExternalLogins { get; set; }

        public LoginViewModel(
            string returnUrl,
            bool registrationEnabled,
            bool passwordRecoveryEnabled,
            bool groupRegistrationEnabled,
            string error,
            List<AuthenticationScheme> externalLogins)
        {
            ReturnUrl = returnUrl;
            RegistrationEnabled = registrationEnabled;
            PasswordRecoveryEnabled = passwordRecoveryEnabled;
            GroupRegistrationEnabled = groupRegistrationEnabled;

            Error = error;

            ExternalLogins = externalLogins;
        }
    }
}
