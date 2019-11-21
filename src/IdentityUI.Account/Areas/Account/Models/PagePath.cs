using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models
{
    public class PagePath
    {
        public const string ACCOUNT_AREA_NAME = "Account";
        public const string ACCOUNT_AREA = "/" + ACCOUNT_AREA_NAME;

        public const string AUTH = ACCOUNT_AREA;

        public const string LOGIN = AUTH + "/Login";
        public const string LOGOUT = AUTH + "/Logout";
        public const string LOGIN_2FA = AUTH + "/LoginWith2fa";
        public const string LOCKOUT = AUTH + "/Lockout";

        public const string REGISTER = AUTH + "/Register";
        public const string REGISTER_SUCCESS = AUTH + "/RegisterSuccess";

        public const string RECOVER_PASSWORD = AUTH + "/RecoverPassword";
        public const string RECOVER_PASSWORD_SUCCESS = AUTH + "/RecoverPasswordSuccess";

        public const string RESET_PASSWORD = AUTH + "/ResetPassword";
        public const string RESET_PASSWORD_SUCCESS = AUTH + "/ResetPasswordSuccess";

        public const string ACCESS_DENIED = AUTH + "/AccessDenied";

        public const string CONFIRME_EMAIL = AUTH + "/ConfirmEmail";

        public const string MANAGE = ACCOUNT_AREA + "/Manage";

        public const string MANAGE_ACCOUNT = MANAGE + "/";
        
        public const string MANAGE_TWO_FACTOR_AUTHENTICATOR = MANAGE + "/TwoFactorAuthenticator";
        public const string MANAGE_ADD_TWO_FACTOR_AUTHENTICATOR = MANAGE + "/AddTwoFactorAuthenticator";
        public const string MANAGE_DISABLE_TWO_FACTOR_AUTHENTICATOR = MANAGE + "/DisableTwoFactorAuthenticator";
        public const string MANAGE_RESET_TWO_FACTOR_AUTHENTICATOR = MANAGE + "/ResetTwoFactorAuthenticator";

        public const string MANAGE_CHANGE_PASSWORD = MANAGE + "/ChangePassword";

        public static string HOME;
    }
}
