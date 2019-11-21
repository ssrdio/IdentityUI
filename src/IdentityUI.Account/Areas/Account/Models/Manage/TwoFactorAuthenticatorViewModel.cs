using SSRD.AdminUI.Template.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Manage
{
    public class TwoFactorAuthenticatorViewModel
    {
        public bool TwoFactorAuthenticationEnabled { get; set; }

        public StatusAlertViewModel StatusAlert { get; set; }

        public TwoFactorAuthenticatorViewModel(bool twoFactorAuthenticationEnabled)
        {
            TwoFactorAuthenticationEnabled = twoFactorAuthenticationEnabled;
        }


        public TwoFactorAuthenticatorViewModel(StatusAlertViewModel statusAlert)
        {
            StatusAlert = statusAlert;
        }
    }
}
