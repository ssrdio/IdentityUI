using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal sealed class GrouPermissionAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private const string GROUP_ROUTE_KEY = "groupId";

        private readonly string _requirePermission;

        public GrouPermissionAuthorizeAttribute(string permission)
        {
            _requirePermission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool isIdentityAdmin = context.HttpContext.User.HasPermission(_requirePermission);
            if (isIdentityAdmin)
            {
                return;
            }

            string logedInUserId = context.HttpContext.User.GetUserId();

            bool isInRequiredRole = context.HttpContext.User.HasGroupPermission(_requirePermission);
            if (!isInRequiredRole)
            {
                context.Result = new ForbidResult();
                return;
            }

            bool groupIdExist = context.RouteData.Values.TryGetValue(GROUP_ROUTE_KEY, out object groupIdObj);
            if(!groupIdExist)
            {
                context.Result = new NotFoundResult();
            }

            string groupId = (string)groupIdObj;

            BaseSpecification<GroupUserEntity> baseSpecification = new BaseSpecification<GroupUserEntity>();
            baseSpecification.AddFilter(x => x.UserId == logedInUserId);
            baseSpecification.AddFilter(x => x.GroupId == groupId);
            baseSpecification.AddFilter(x => x.Role.Permissions.Any(c => c.Permission.Name.ToUpper() == _requirePermission.ToUpper())); //TODO: check this

            IBaseRepository<GroupUserEntity> groupUserRepository = context.HttpContext.RequestServices.GetService<IBaseRepository<GroupUserEntity>>();

            bool groupUserExist = groupUserRepository.Exist(baseSpecification);
            if (!groupUserExist)
            {
                //_logger.LogError($"User does not have permission for group. UserId {logedInUserId}, {groupId}");
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
