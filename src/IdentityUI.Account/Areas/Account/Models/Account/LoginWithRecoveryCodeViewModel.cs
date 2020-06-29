using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Account
{
    public class LoginWithRecoveryCodeViewModel
    {
        public string ReturnUrl { get; set; }

        public string RecoveryCode { get; set; }

        public LoginWithRecoveryCodeViewModel(string returnUrl)
        {
            ReturnUrl = returnUrl;
        }
    }
}
