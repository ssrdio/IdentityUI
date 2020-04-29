using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Group.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Group
{
    public interface IGroupUserService
    {
        Result AddExisting(string groupId, AddExistingUserRequest addExistingUserRequest);
        Task<Result> Invite(string groupId, InviteUserToGroupRequest inviteUserToGroup);

        Result ChangeRole(long groupUserId, string roleId, string logedInUserId, string logedInUserGrroupId, bool hasGlobalPermission);
        Result Remove(long groupUserId, string loggedInUserId, string loggedInUserGroupId, bool hasGlobalPermissions);
        Result Leave(string userId, string groupId);

        List<RoleListData> CanManageRoles(string userId, string groupId, bool isGlobal);
    }
}
