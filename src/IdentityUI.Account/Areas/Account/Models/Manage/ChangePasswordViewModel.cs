using SSRD.AdminUI.Template.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Manage
{
    public class ChangePasswordViewModel
    {
        [DisplayName("Old password")]
        public string OldPassword { get; set; }
        [DisplayName("New password")]
        public string NewPassword { get; set; }
        [DisplayName("Confirm new password")]
        public string ConfirmNewPassword { get; set; }

        public StatusAlertViewModel StatusAlert { get; set; }


        public ChangePasswordViewModel(StatusAlertViewModel statusAlert)
        {
            StatusAlert = statusAlert;
        }
    }
}
