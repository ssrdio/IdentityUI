using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Account
{
    public class LoginWith2faViewModel
    {
        public string Code { get; set; }
        [DisplayName("Remember me")]
        public bool RememberMe { get; set; }
        [DisplayName("Remember machine")]
        public bool RememberMachine { get; set; }

        public LoginWith2faViewModel(bool rememberMe)
        {
            RememberMe = rememberMe;
        }
    }
}
