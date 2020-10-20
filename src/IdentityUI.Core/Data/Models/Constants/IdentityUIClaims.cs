using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Models.Constants
{
    public static class IdentityUIClaims
    {
        public const string SESSION_CODE = "IdentityUI.SessionCode";

        public const string GROUP_ID = "IdentityUI.Group.Id";
        public const string GROUP_NAME = "IdentityUI.Group.Name";
        public const string GROUP_ROLE = "IdentityUI.Group.Role";
        public const string GROUP_PERMISSION = "IdentityUI.Group.Permission";

        public const string PERMISSION = "IdentityUI.Permission";

        public const string IMPERSONATOR_ID = "IdentityUI.ImpersonatorId";
        public const string IMPERSONATOR_NAME = "IdentityUI.ImpersonatorName";
        public const string IMPERSONATOR_EMAIL = "IdentityUI.ImpersonatorEmail";

        public const string IMPERSONATOR_ROLE = "IdentityUI.ImpersonatorRole";
        public const string IMPERSONATOR_PERMISSION = "IdentityUI.ImpersonatorPermission";
    }
}
