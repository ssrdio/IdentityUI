using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models
{
    public static class PagePath
    {
        public const string IDENTITY_ADMIN_AREA_NAME = "IdentityAdmin";

        public const string IDENTITY_ADMIN_AREA = "/" + IDENTITY_ADMIN_AREA_NAME;

        public const string DASHBOARD = IDENTITY_ADMIN_AREA + "/";

        public const string USER = IDENTITY_ADMIN_AREA + "/User";

        public const string USER_DETAILS = USER + "/Details/{0}";
        public const string USER_CREDENTIALS = USER + "/Credentials/{0}";
        public const string USER_ROLES = USER + "/Roles/{0}";
        public const string USER_SESSIONS = USER + "/Sessions/{0}";

        public const string USER_NEW = USER + "/New";

        public const string ROLE = IDENTITY_ADMIN_AREA + "/Role";

        public const string ROLE_DETAILS = ROLE + "/Details/{0}";
        public const string ROLE_USERS = ROLE + "/Users/{0}";

        public const string ROLE_NEW = ROLE + "/New";

        public const string CLIENT = IDENTITY_ADMIN_AREA + "/Client";

        public const string CLIENT_NEW = CLIENT + "/New";
        public const string CLIENT_DETAILS = CLIENT + "/Details/{0}";

        public static string LOGOUT;
        public static string MANAGE;
        public static string HOME;
    }
}
