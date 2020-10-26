using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Account
{
    public class RegisterGroupViewModel
    {
        public string GroupName { get; set; }

        public RegisterBaseGroupUserViewModel BaseUser { get; set; }

        public bool RecoverPasswordEnabled { get; set; }

        public RegisterGroupViewModel(bool recoverPasswordEnabled)
        {
            RecoverPasswordEnabled = recoverPasswordEnabled;
        }
    }
}
