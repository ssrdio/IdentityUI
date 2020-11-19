using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Services.Identity;
using System;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AuthorizeIdentityAdminAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            IIdentityUIUserInfoService identityUIUserInfoService = context.HttpContext.RequestServices.GetRequiredService<IIdentityUIUserInfoService>();

            bool hasRole = identityUIUserInfoService.HasRole(IdentityUIRoles.IDENTITY_MANAGMENT_ROLE);
            if(hasRole)
            {
                return;
            }

            context.Result = new ForbidResult();
        }
    }
}
