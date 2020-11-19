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
        public const string USER_ATTRIBUTES = USER + "/{0}/Attributes";

        public const string USER_NEW = USER + "/New";

        public const string ROLE = IDENTITY_ADMIN_AREA + "/Role";

        public const string ROLE_DETAILS = ROLE + "/Details/{0}";
        public const string ROLE_USERS = ROLE + "/Users/{0}";
        public const string ROLE_ASSIGNMENTS = ROLE + "/Assignments/{0}";
        public const string ROLE_PERMISSIONS = ROLE + "/Permissions/{0}";

        public const string ROLE_NEW = ROLE + "/New";

        public const string PERMISSION = IDENTITY_ADMIN_AREA + "/Permission";
        public const string PERMISSION_DETAILS = PERMISSION + "/Details/{0}";
        public const string PERMISSION_ROLES = PERMISSION + "/Roles/{0}";

        public const string GROUP = IDENTITY_ADMIN_AREA + "/Group";
        public const string GROUP_USERS = GROUP + "/Users/{0}";
        public const string GROUP_ATTRIBUTES = GROUP + "/Attributes/{0}";
        public const string GROUP_INVITES = GROUP + "/Invites/{0}";

        public const string INVITE = IDENTITY_ADMIN_AREA + "/Invite";

        public const string SETTING = IDENTITY_ADMIN_AREA + "/Setting";
        public const string SETTING_EMAIL = SETTING + "/Email";
        public const string SETTING_EMAIL_DETAILS = SETTING_EMAIL + "/Details/{0}";

        public const string AUDIT = IDENTITY_ADMIN_AREA + "/Audit";

        [Obsolete("Use IdentityUIEndpoints.ProfileImage")]
        public const string PROFILE_IMAGE = "/Account/Manage/GetProfileImage";

        [Obsolete("Use IdentityUIEndpoints.Logout")]
        public static string LOGOUT;

        [Obsolete("Use IdentityUIEndpoints.Manage")]
        public static string MANAGE;

        [Obsolete("Use IdentityUIEndpoints.Home")]
        public static string HOME;
    }
}
