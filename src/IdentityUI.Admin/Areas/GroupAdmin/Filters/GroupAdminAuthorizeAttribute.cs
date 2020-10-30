using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Services.Identity;
using System;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class GroupAdminAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool hasGroupPermission = context.HttpContext.HasGroupPermission(IdentityUIPermissions.GROUP_ADMIN_ACCESS);
            if (hasGroupPermission)
            {
                return;
            }

            bool isImpersonized = context.HttpContext.IsImpersonized();
            if (isImpersonized)
            {
                bool impersonizerHasPermission = context.HttpContext.ImpersonizerHasRole(IdentityUIPermissions.GROUP_ADMIN_ACCESS);
                if (impersonizerHasPermission)
                {
                    return;
                }
            }
        }
    }
}
