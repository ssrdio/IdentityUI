using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Setting.Email
{
    public class EmailIndexViewModel
    {
        public bool UseEmailSender { get; set; }

        public EmailIndexViewModel(bool useEmailSender)
        {
            UseEmailSender = useEmailSender;
        }
    }
}
