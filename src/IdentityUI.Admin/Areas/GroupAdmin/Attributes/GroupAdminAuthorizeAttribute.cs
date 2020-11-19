using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Services.Identity;
using System;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class GroupAdminAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _requiredPermission;

        public GroupAdminAuthorizeAttribute()
        {

        }

        public GroupAdminAuthorizeAttribute(string permission)
        {
            _requiredPermission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string groupId = context.HttpContext.GetGroupId();
            if(string.IsNullOrEmpty(groupId))
            {
                context.Result = new ForbidResult();
                return;
            }

            bool hasGroupPermission = context.HttpContext.HasGroupPermissionOrImpersonatorHasPermission(IdentityUIPermissions.GROUP_ADMIN_ACCESS);
            if (!hasGroupPermission)
            {
                context.Result = new ForbidResult();
                return;
            }

            if(string.IsNullOrEmpty(_requiredPermission))
            {
                return;
            }

            bool hasRequiredGroupPermission = context.HttpContext.HasGroupPermissionOrImpersonatorHasPermission(_requiredPermission);
            if (!hasRequiredGroupPermission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
