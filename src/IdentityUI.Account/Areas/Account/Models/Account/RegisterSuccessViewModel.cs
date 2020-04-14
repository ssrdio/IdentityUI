using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Account
{
    public class RegisterSuccessViewModel
    {
        public bool UseEmailSender { get; set; }

        public RegisterSuccessViewModel(bool useEmailSender)
        {
            UseEmailSender = useEmailSender;
        }
    }
}
