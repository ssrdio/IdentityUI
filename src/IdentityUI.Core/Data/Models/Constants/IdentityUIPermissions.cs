using System;
using System.Collections.Generic;
using System.Text;

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

        public static readonly string[] ALL_GROUP = new string[]
        {
            GROUP_CAN_SEE_USERS,
            GROUP_CAN_INVITE_USERS,
            GROUP_CAN_MANAGE_INVITES,
            GROUP_CAN_ADD_EXISTING_USERS,
            GROUP_CAN_MANAGE_ROLES,
            GROUP_CAN_REMOVE_USERS,
            GROUP_CAN_MANAGE_ATTRIBUTES,
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
                description: "User can view, add, edit group attributes")
        };
    }
}
