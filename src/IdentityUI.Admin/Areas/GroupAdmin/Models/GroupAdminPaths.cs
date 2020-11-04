namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models
{
    public static class GroupAdminPaths
    {
        public const string GROUP_ADMIN_AREA_NAME = "GroupAdmin";

        public const string GROUP_ADMIN_AREA = "/" + GROUP_ADMIN_AREA_NAME;

        public const string DASHBOARD = GROUP_ADMIN_AREA + "/{0}/Dashboard";
        
        public const string USER = GROUP_ADMIN_AREA + "/{0}/User";

        public const string INVITE = GROUP_ADMIN_AREA + "/{0}/Invite";
        
        public const string ATTRIBUTE = GROUP_ADMIN_AREA + "/{0}/Attribute";

        public const string AUDIT = GROUP_ADMIN_AREA + "/{0}/Audit";

        public const string SETTINGS = GROUP_ADMIN_AREA + "/{0}/Settings";
    }
}
