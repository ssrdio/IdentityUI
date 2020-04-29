using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Models.Constants
{
    public static class IdentityUIRoles
    {
        public const string IDENTITY_MANAGMENT_ROLE = "IdentitySuperAdmin";

        public static readonly string[] ALL_ROLES = new string[]
        {
            IDENTITY_MANAGMENT_ROLE,
        };
    }
}
