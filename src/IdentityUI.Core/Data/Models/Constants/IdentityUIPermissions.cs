using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Models.Constants
{
    public static class IdentityUIPermissions
    {
        public const string GROUP_CAN_SEE_USERS = "group_can_see_users";
        public const string GROUP_CAN_INVITE_USERS = "group_can_invite_users";
        public const string GROUP_CAN_ADD_EXISTING_USERS = "group_can_add_existing_users";
        public const string GROUP_CAN_MANAGE_ROLES = "group_can_manage_roles";
        public const string GROUP_CAN_REMOVE_USERS = "group_can_remove_users";
        public const string GROUP_CAN_MANAGE_ATTRIBUTES = "group_can_manage_attributes";

        public static readonly string[] ALL_PERMISSIONS = new string[]
        {
            GROUP_CAN_SEE_USERS,
            GROUP_CAN_INVITE_USERS,
            GROUP_CAN_ADD_EXISTING_USERS,
            GROUP_CAN_MANAGE_ROLES,
            GROUP_CAN_REMOVE_USERS,
            GROUP_CAN_MANAGE_ATTRIBUTES,
        };
    }
}
