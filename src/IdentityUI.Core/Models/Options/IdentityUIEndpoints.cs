using System;

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
        public string AcceptInvite { get; set; } = "/Account/AcceptInvite";

        public string ProfileImage { get; set; } = "/Account/Manage/GetProfileImage";
        public string AdminLogo { get; set; } = "/adminUI/template/images/logo.png";
        public string AccountSettingsLogo { get; set; } = "/adminUI/template/images/logo.png";

        public bool RegisterEnabled { get; set; } = true;
        public bool GroupRegistrationEnabled { get; set; } = false;

        public string AuthenticatorIssuer { get; set; } = "IdentityUI";

        /// <summary>
        /// If you set IdentityUI.EmailSender in the appsettings this will automatically change to true if not it will change to false. If you are using your
        /// own implementation of <see cref="Microsoft.AspNetCore.Identity.UI.Services.IEmailSender"/> you need to set this to true.
        /// </summary>
        public bool? UseEmailSender { get; set; } = null;

        public bool UseSmsGateway { get; set; } = false;

        public int NumberOfRecoveryCodes { get; set; } = 6;
        public TimeSpan InviteValidForTimeSpan { get; set; } = TimeSpan.FromDays(7);

        public bool BypassTwoFactorOnExternalLogin { get; set; } = false;

        /// <summary>
        /// If this is set to false you need to provide username in /Account/Register, /Account/AcceptInvite, /Account/ExternalLoginRegister requests
        /// </summary>
        public bool UseEmailAsUsername { get; set; } = true;

        /// <summary>
        /// If user can see his own audit
        /// </summary>
        public bool ShowAuditToUser { get; set; } = false;

        public bool CanChangeGroupName { get; set; } = true;
        public bool CanRemoveGroup { get; set; } = true;

        public bool CanRemoveUser { get; set; } = true;

        public bool AllowImpersonation { get; set; } = true;
    }
}
