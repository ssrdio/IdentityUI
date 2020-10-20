using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Services.Identity;
using System;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AuthorizeIdentityAdminAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool hasRole = context.HttpContext.HasRole(IdentityUIRoles.IDENTITY_MANAGMENT_ROLE);
            if(hasRole)
            {
                return;
            }

            bool isImpersonized = context.HttpContext.IsImpersonized();
            if(isImpersonized)
            {
                bool impersonizerHasRole = context.HttpContext.ImpersonizerHasRole(IdentityUIRoles.IDENTITY_MANAGMENT_ROLE);
                if(impersonizerHasRole)
                {
                    return;
                }
            }

            context.Result = new ForbidResult();
        }
    }
}
