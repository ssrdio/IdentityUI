using SSRD.AdminUI.Template.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Manage
{
    public class CredentailsViewModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }

        public bool HasPassword { get; set; }

        public bool HasExternalLoginProvider { get; set; }
        public string ExternalLoginProvider { get; set; }

        public StatusAlertViewModel StatusAlert { get; set; }

        public CredentailsViewModel(bool hasPassword, bool hasExternalLoginProvider, string externalLoginProvider)
        {
            HasPassword = hasPassword;
            HasExternalLoginProvider = hasExternalLoginProvider;
            ExternalLoginProvider = externalLoginProvider;
        }

        public CredentailsViewModel(StatusAlertViewModel statusAlert)
        {
            StatusAlert = statusAlert;
        }
    }
}
