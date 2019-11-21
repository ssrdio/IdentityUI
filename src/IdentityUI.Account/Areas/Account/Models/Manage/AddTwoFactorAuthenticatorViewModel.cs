using SSRD.AdminUI.Template.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Manage
{
    public class AddTwoFactorAuthenticatorViewModel
    {
        [DisplayName("Shared key")]
        public string SharedKey { get; set; }
        [DisplayName("Authenticator uri")]
        public string AuthenticatorUri { get; set; }
        [DisplayName("Verefication code")]
        public string VereficationCode { get; set; }
        public StatusAlertViewModel StatusAlert { get; set; }

        public AddTwoFactorAuthenticatorViewModel(string sharedKey, string authenticationUri)
        {
            SharedKey = sharedKey;
            AuthenticatorUri = authenticationUri;
        }
        public AddTwoFactorAuthenticatorViewModel(StatusAlertViewModel statusAlert)
        {
            StatusAlert = statusAlert;
        }
    }
}
