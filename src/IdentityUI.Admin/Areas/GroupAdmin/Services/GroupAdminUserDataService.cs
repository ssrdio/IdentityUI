using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.User;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Services
{
    public class GroupAdminUserDataService : IGroupAdminUserDataService
    {
        private readonly IGroupUserStore _groupUserStore;

        public GroupAdminUserDataService(IGroupUserStore groupUserStore)
        {
            _groupUserStore = groupUserStore;
        }

        public Task<Result<GroupAdminUserDetailsModel>> Get(long groupUserId)
        {
            IBaseSpecification<GroupUserEntity, GroupAdminUserDetailsModel> specification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => x.Id == groupUserId)
                .Select(x => new GroupAdminUserDetailsModel(
                    x.User.Id,
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

        public Task<Result<GroupAdminUserDetailsViewModel>> GetDetailsViewModel(string groupId, long groupUserId)
        {
            GroupAdminUserDetailsViewModel viewModel = new GroupAdminUserDetailsViewModel(
                groupId: groupId,
                groupUserId: groupUserId);

            return Task.FromResult(Result.Ok(viewModel));
        }

        public async Task<Result<GroupAdminUserIndexViewModel>> GetIndexViewModel(string groupId)
        {
            List<RoleListData> canAssigneRoles = await _groupUserStore.CanAssigneRoles();
            List<RoleListData> canManageRoles = await _groupUserStore.CanManageRoles();
            bool canChangeOwnRole = _groupUserStore.CanChangeOwnRole();

            GroupAdminUserIndexViewModel viewModel = new GroupAdminUserIndexViewModel(
                groupId: groupId,
                canMangeGroupRoles: canManageRoles,
                canAssigneGroupRoles: canAssigneRoles,
                canChangeOwnRole: canChangeOwnRole);

            return Result.Ok(viewModel);
        }
    }
}
