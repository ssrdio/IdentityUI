using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models
{
    public static class GroupAdminPaths
    {
        public const string GROUP_ADMIN_AREA_NAME = "GroupAdmin";

        public const string GROUP_ADMIN_AREA = "/" + GROUP_ADMIN_AREA_NAME;

        public const string DASHBOARD = GROUP_ADMIN_AREA + "/Dashboard";
        
        public const string USER = GROUP_ADMIN_AREA + "/User";

        public const string INVITE = GROUP_ADMIN_AREA + "/Invite";
        
        public const string ATTRIBUTE = GROUP_ADMIN_AREA + "/Attribute";

        public const string AUDIT = GROUP_ADMIN_AREA + "/Audit";

        public const string PROFILE_IMAGE = "/Account/Manage/GetProfileImage";

        public static string LOGOUT;
        public static string MANAGE;
        public static string HOME;
    }
}
