using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Models.Options
{
    public class IdentityUIEndpoints
    {
        public string Home { get; set; } = "/";

        public string Login { get; set; } = "/Account/Login/";
        public string Logout { get; set; } = "/Account/Logout/";
        public string AccessDenied { get; set; } = "/Account/AccessDenied/";

        public string Manage { get; set; } = "/Account/Manage/";

        public string ConfirmeEmail { get; set; } = "/Account/ConfirmEmail";
        public string ResetPassword { get; set; } = "/Account/ResetPassword";

        public bool RegisterEnabled { get; set; } = true;

        /// <summary>
        /// If you set IdentityUI.EmailSender in the appsettings this will automaticly change to true if not it will change to false. If you are using your
        /// own implementation of <see cref="Microsoft.AspNetCore.Identity.UI.Services.IEmailSender"/> you need to set this to true.
        /// </summary>
        public bool UseEmailSender { get; set; } = false;
    }
}
