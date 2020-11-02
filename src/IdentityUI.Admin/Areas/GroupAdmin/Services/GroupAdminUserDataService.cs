using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Audit;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.User;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Interfaces;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Services.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Services
{
    public class GroupAdminUserDataService : IGroupAdminUserDataService
    {
        private readonly IGroupUserStore _groupUserStore;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IdentityUIClaimOptions _identityUIClaimOptions;

        public GroupAdminUserDataService(
            IGroupUserStore groupUserStore,
            IHttpContextAccessor httpContextAccessor,
            IOptions<IdentityUIClaimOptions> identityUIClaimOptions)
        {
            _groupUserStore = groupUserStore;
            _httpContextAccessor = httpContextAccessor;
            _identityUIClaimOptions = identityUIClaimOptions.Value;
        }

        public Task<Result<GroupAdminUserDetailsModel>> Get(long groupUserId)
        {
            IBaseSpecification<GroupUserEntity, GroupAdminUserDetailsModel> specification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => x.Id == groupUserId)
                .Select(x => new GroupAdminUserDetailsModel(
                    x.Id,
                    x.User.UserName,
                    x.User.Email,
                    x.User.FirstName,
                    x.User.LastName,
                    x.User.PhoneNumber,
                    x.User.EmailConfirmed,
                    x.User.PhoneNumberConfirmed,
                    x.User.TwoFactorEnabled,
                    x.User.Enabled,
                    x.User.LockoutEnd.HasValue ? x.User.LockoutEnd.Value.ToString("o") : null))
                .Build();

            return _groupUserStore.SingleOrDefault(specification);
        }

        public Task<Result<GroupAdminUserDetailsViewModel>> GetDetailsViewModel(long groupUserId)
        {
            IBaseSpecification<GroupUserEntity, GroupAdminUserDetailsViewModel> specification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => x.Id == groupUserId)
                .Select(x => new GroupAdminUserDetailsViewModel(
                    x.Id,
                    x.User.UserName,
                    x.User.Email,
                    x.User.FirstName,
                    x.User.LastName,
                    x.User.EmailConfirmed,
                    x.User.PhoneNumber,
                    x.User.PhoneNumberConfirmed,
                    x.User.TwoFactorEnabled,
                    x.User.Enabled,
                    x.User.LockoutEnd.HasValue ? x.User.LockoutEnd.Value.ToString("o") : null))
                .Build();

            return _groupUserStore.SingleOrDefault(specification);
        }

        public async Task<Result<GroupAdminUserIndexViewModel>> GetIndexViewModel()
        {
            string groupId = _httpContextAccessor.HttpContext.User.GetGroupId(_identityUIClaimOptions);
            List<RoleListData> canAssigneRoles = await _groupUserStore.CanAssigneRoles();
            List<RoleListData> canManageRoles = await _groupUserStore.CanManageRoles();
            //bool canChangeOwnRole = _groupUserStore.CanChangeOwnRole();
            bool canChangeOwnRole = true;

            GroupAdminUserIndexViewModel viewModel = new GroupAdminUserIndexViewModel(
                groupId: groupId,
                canMangeGroupRoles: canManageRoles,
                canAssigneGroupRoles: canAssigneRoles,
                canChangeOwnRole: canChangeOwnRole);

            return Result.Ok(viewModel);
        }
    }
}
