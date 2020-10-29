using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SSRD.IdentityUI.Core.Services.Identity;
using System;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal sealed class HasPermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _permission;

        public HasPermissionAttribute(string permission)
        {
            _permission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool hasPermission = context.HttpContext.User.HasPermission(_permission);
            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
