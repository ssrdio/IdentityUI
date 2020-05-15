using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Models.Constants
{
    public static class IdentityUIRoles
    {
        public const string IDENTITY_MANAGMENT_ROLE = "IdentitySuperAdmin";

        public static readonly string[] ALL = new string[]
        {
            IDENTITY_MANAGMENT_ROLE,
        };

        internal static readonly List<RoleSeedModel> ALL_DATA = new List<RoleSeedModel>
        {
            new RoleSeedModel(
                name: IDENTITY_MANAGMENT_ROLE,
                type: Enums.Entity.RoleTypes.Global,
                description: "Users with this permission can access to IdentityAdmin",
                permissions: IdentityUIPermissions.ALL.ToList())
        };
    }
}
