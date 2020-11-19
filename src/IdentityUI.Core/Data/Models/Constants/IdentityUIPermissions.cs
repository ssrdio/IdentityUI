using System.Collections.Generic;

namespace SSRD.IdentityUI.Core.Data.Models.Constants
{
    public static class IdentityUIPermissions
    {
        public const string IDENTITY_UI_CAN_MANAGE_GROUPS = "identity_ui_can_manage_groups";

        public const string GROUP_CAN_SEE_USERS = "group_can_see_users";
        public const string GROUP_CAN_INVITE_USERS = "group_can_invite_users";
        public const string GROUP_CAN_MANAGE_INVITES = "group_can_manage_invites";
        public const string GROUP_CAN_ADD_EXISTING_USERS = "group_can_add_existing_users";
        public const string GROUP_CAN_MANAGE_ROLES = "group_can_manage_roles";
        public const string GROUP_CAN_REMOVE_USERS = "group_can_remove_users";
        public const string GROUP_CAN_MANAGE_ATTRIBUTES = "group_can_manage_attributes";

        public const string GROUP_ADMIN_ACCESS = "group_admin_access";
        public const string GROUP_CAN_ACCESS_DASHBOARD = "group_can_access_dashboard";
        public const string GROUP_CAN_VIEW_AUDIT = "group_can_view_audit";

        public const string GROUP_CAN_ACCESS_USER_DETAILS = "group_can_access_user_details";
        public const string GROUP_CAN_MANAGE_USER_DETAILS = "group_can_manage_user_details";
        public const string GROUP_CAN_IMPERSONATE_USER = "group_can_impersonate_user";

        public const string GROUP_CAN_MANAGE_SETTINGS = "group_can_manage_settings";

        public static readonly string[] ALL_GROUP = new string[]
        {
            GROUP_CAN_SEE_USERS,
            GROUP_CAN_INVITE_USERS,
            GROUP_CAN_MANAGE_INVITES,
            GROUP_CAN_ADD_EXISTING_USERS,
            GROUP_CAN_MANAGE_ROLES,
            GROUP_CAN_REMOVE_USERS,
            GROUP_CAN_MANAGE_ATTRIBUTES,

            GROUP_ADMIN_ACCESS,
            GROUP_CAN_ACCESS_DASHBOARD,
            GROUP_CAN_VIEW_AUDIT,

            GROUP_CAN_ACCESS_USER_DETAILS,
            GROUP_CAN_MANAGE_USER_DETAILS,
            GROUP_CAN_IMPERSONATE_USER,

            GROUP_CAN_MANAGE_SETTINGS,
        };

        public static readonly string[] ALL = new string[]
        {
            IDENTITY_UI_CAN_MANAGE_GROUPS,

            GROUP_CAN_SEE_USERS,
            GROUP_CAN_INVITE_USERS,
            GROUP_CAN_MANAGE_INVITES,
            GROUP_CAN_ADD_EXISTING_USERS,
            GROUP_CAN_MANAGE_ROLES,
            GROUP_CAN_REMOVE_USERS,
            GROUP_CAN_MANAGE_ATTRIBUTES,

            GROUP_ADMIN_ACCESS,
            GROUP_CAN_ACCESS_DASHBOARD,
            GROUP_CAN_VIEW_AUDIT,

            GROUP_CAN_ACCESS_USER_DETAILS,
            GROUP_CAN_MANAGE_USER_DETAILS,
            GROUP_CAN_IMPERSONATE_USER,

            GROUP_CAN_MANAGE_SETTINGS,
        };

        internal static readonly List<PermissionSeedModel> ALL_DATA = new List<PermissionSeedModel>
        {
            new PermissionSeedModel(
                name: IDENTITY_UI_CAN_MANAGE_GROUPS,
                description: "User can see all groups, can add new groups"),
            new PermissionSeedModel(
                name: GROUP_CAN_SEE_USERS,
                description: "User can see other members in group"),
            new PermissionSeedModel(
                name: GROUP_CAN_INVITE_USERS,
                description: "User can invite members to group"),
            new PermissionSeedModel(
                name: GROUP_CAN_MANAGE_INVITES,
                description: "User can see all group invites and removed them"),
            new PermissionSeedModel(
                name: GROUP_CAN_ADD_EXISTING_USERS,
                description: "User can add users to group that already exists on IdentityUI"),
            new PermissionSeedModel(
                name: GROUP_CAN_MANAGE_ROLES,
                description: "User can manage roles. When this permission is used in a role with type 'group' user can only mange roles that are assigned to that role"),
            new PermissionSeedModel(
                name: GROUP_CAN_REMOVE_USERS,
                description: "User can remove users from group. When this permission is used in a role with type 'group' user can only remove users that have a role that is assigned to that role"),
            new PermissionSeedModel(
                name: GROUP_CAN_MANAGE_ATTRIBUTES,
                description: "User can view, add, edit group attributes"),
            new PermissionSeedModel(
                name: GROUP_ADMIN_ACCESS,
                description: "User has access to GroupAdmin"),
            new PermissionSeedModel(
                name: GROUP_CAN_ACCESS_DASHBOARD,
                description: "User has access to group dashboard"),
            new PermissionSeedModel(
                name: GROUP_CAN_VIEW_AUDIT,
                description: "User can view audit logs for the group"),
            new PermissionSeedModel(
                name: GROUP_CAN_ACCESS_USER_DETAILS,
                description: "User can access another user details"),
            new PermissionSeedModel(
                name: GROUP_CAN_MANAGE_USER_DETAILS,
                description: "User can manage another user details"),
            new PermissionSeedModel(
                name: GROUP_CAN_IMPERSONATE_USER,
                description: "User can impersonate another user"),
            new PermissionSeedModel(
                name: GROUP_CAN_MANAGE_SETTINGS,
                description: "User can view/change group settings"),
        };
    }
}
