using System.Collections.Generic;
using System.Linq;

namespace SSRD.IdentityUI.Core.Data.Models.Constants
{
    public static class IdentityUIRoles
    {
        public const string IDENTITY_MANAGMENT_ROLE = "IdentitySuperAdmin";

        public const string GROUP_ADMIN = "GroupAdmin";

        public static readonly string[] ALL = new string[]
        {
            IDENTITY_MANAGMENT_ROLE,
        };

        internal static readonly List<RoleSeedModel> ALL_DATA = new List<RoleSeedModel>
        {
            new RoleSeedModel(
                name: IDENTITY_MANAGMENT_ROLE,
                type: Enums.Entity.RoleTypes.Global,
                description: "Users with this role can access to IdentityAdmin",
                permissions: IdentityUIPermissions.ALL.ToList()),
            new RoleSeedModel(
                name: GROUP_ADMIN,
                type: Enums.Entity.RoleTypes.Group,
                description: "Users with this role can access GroupAdmin. Users that register a group get this role for that role",
                permissions: new List<string>
                {
                    IdentityUIPermissions.GROUP_CAN_INVITE_USERS,
                    IdentityUIPermissions.GROUP_CAN_MANAGE_INVITES,
                    IdentityUIPermissions.GROUP_CAN_MANAGE_ROLES,
                    IdentityUIPermissions.GROUP_CAN_REMOVE_USERS,
                    IdentityUIPermissions.GROUP_CAN_SEE_USERS,
                    IdentityUIPermissions.GROUP_CAN_ACCESS_DASHBOARD,
                    IdentityUIPermissions.GROUP_CAN_MANAGE_ATTRIBUTES,
                    IdentityUIPermissions.GROUP_CAN_MANAGE_SETTINGS,
                    IdentityUIPermissions.GROUP_CAN_VIEW_AUDIT,
                    IdentityUIPermissions.GROUP_CAN_ACCESS_USER_DETAILS,
                    IdentityUIPermissions.GROUP_CAN_MANAGE_USER_DETAILS,
                    IdentityUIPermissions.GROUP_CAN_IMPERSONATE_USER
                }),
        };
    }
}
