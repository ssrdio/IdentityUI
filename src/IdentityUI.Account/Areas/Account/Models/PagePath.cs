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
        public const string LOGIN_WITH_RECOVERY_CODE = AUTH + "/LoginWithRecoveryCode";
        public const string LOCKOUT = AUTH + "/Lockout";

        public const string REGISTER = AUTH + "/Register";
        public const string REGISTER_GROUP = AUTH + "/RegisterGroup";
        public const string REGISTER_SUCCESS = AUTH + "/RegisterSuccess";

        public const string RECOVER_PASSWORD = AUTH + "/RecoverPassword";
        public const string RECOVER_PASSWORD_SUCCESS = AUTH + "/RecoverPasswordSuccess";

        public const string RESET_PASSWORD = AUTH + "/ResetPassword";
        public const string RESET_PASSWORD_SUCCESS = AUTH + "/ResetPasswordSuccess";

        public const string ACCESS_DENIED = AUTH + "/AccessDenied";

        public const string CONFIRME_EMAIL = AUTH + "/ConfirmEmail";

        public const string MANAGE = ACCOUNT_AREA + "/Manage";

        public const string MANAGE_ACCOUNT = MANAGE + "/";

        public const string TWO_FACTOR_AUTHENTICATION = "/" + ACCOUNT_AREA_NAME + "/TwoFactorAuthentication";
        public const string ADD_TWO_FACTOR_AUTHENTICATOR = TWO_FACTOR_AUTHENTICATION + "/AddTwoFactorAuthenticator";
        public const string ADD_PHONE_TWO_FACTOR_AUTHENTICATION = TWO_FACTOR_AUTHENTICATION + "/AddTwoFactorPhoneAuthentication";
        public const string ADD_EMAIL_TWO_FACTOR_AUTHENTICATION = TWO_FACTOR_AUTHENTICATION + "/AddTwoFactorEmailAuthentication";
        public const string DISABLE_TWO_FACTOR_AUTHENTICATION = TWO_FACTOR_AUTHENTICATION + "/DisableTwoFactorAuthentication";
        public const string GENERATE_RECOVERY_CODES = TWO_FACTOR_AUTHENTICATION + "/RecoveryCodesView";

        public const string MANAGE_CREDENTIALS = MANAGE + "/Credentials";
        public const string REMOVE_EXTERNAL_PROVIDER = MANAGE_CREDENTIALS + "/RemoveExternalProvider";

        public const string PROFILE_IMAGE = MANAGE + "/GetProfileImage";

        public const string AUDIT = ACCOUNT_AREA + "/Audit";

        public static string HOME;
    }
}
