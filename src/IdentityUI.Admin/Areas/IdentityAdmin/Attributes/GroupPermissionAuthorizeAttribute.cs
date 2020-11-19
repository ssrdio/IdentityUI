using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Services.Identity;
using System;
using System.Linq;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal sealed class GroupPermissionAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private const string GROUP_ROUTE_KEY = "groupId";

        private readonly string _requirePermission;

        public GroupPermissionAuthorizeAttribute(string permission) : base()
        {
            _requirePermission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            IIdentityUIUserInfoService identityUIUserInfoService = context.HttpContext.RequestServices.GetRequiredService<IIdentityUIUserInfoService>();

            bool isIdentityAdmin = identityUIUserInfoService.HasPermission(_requirePermission);
            if (isIdentityAdmin)
            {
                return;
            }

            bool isInRequiredRole = identityUIUserInfoService.HasGroupPermission(_requirePermission);
            if (!isInRequiredRole)
            {
                context.Result = new ForbidResult();
                return;
            }

            bool groupIdExist = context.RouteData.Values.TryGetValue(GROUP_ROUTE_KEY, out object groupIdObj);
            if (!groupIdExist)
            {
                context.Result = new NotFoundResult();
            }

            string groupId = (string)groupIdObj;
            string logedInUserId = context.HttpContext.User.GetUserId();


            BaseSpecification<GroupUserEntity> baseSpecification = new BaseSpecification<GroupUserEntity>();
            baseSpecification.AddFilter(x => x.UserId == logedInUserId);
            baseSpecification.AddFilter(x => x.GroupId == groupId);
            baseSpecification.AddFilter(x => x.Role.Permissions.Any(c => c.Permission.Name.ToUpper() == _requirePermission.ToUpper()));

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
