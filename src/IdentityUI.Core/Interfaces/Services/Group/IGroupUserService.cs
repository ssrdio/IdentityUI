using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Services.Group.Models;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Group
{
    public interface IGroupUserService
    {
        Models.Result.Result AddExisting(string groupId, AddExistingUserRequest addExistingUserRequest);

        Models.Result.Result ChangeRole(long groupUserId, string roleId, string userId);
        Models.Result.Result Remove(long groupUserId);
        Models.Result.Result Leave(string userId, string groupId);

        Task<Result> AddUserToGroup(string userId, string groupId, RoleEntity roleEntity);
    }
}
