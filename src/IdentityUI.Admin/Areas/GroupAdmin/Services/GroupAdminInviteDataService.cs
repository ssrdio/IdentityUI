using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Invite;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Services
{
    public class GroupAdminInviteDataService : IGroupAdminInviteDataService
    {
        private readonly IGroupUserStore _groupUserStore;

        public GroupAdminInviteDataService(IGroupUserStore groupUserStore)
        {
            _groupUserStore = groupUserStore;
        }

        public async Task<Result<GroupAdminInviteViewModel>> GetInviteViewModel(string groupId)
        {
            List<RoleListData> canAssigneRoles = await _groupUserStore.CanAssigneRoles();

            GroupAdminInviteViewModel groupAdminInviteViewModel = new GroupAdminInviteViewModel(
                groupId: groupId,
                canAssignRoles: canAssigneRoles);

            return Result.Ok(groupAdminInviteViewModel);
        }
    }
}
