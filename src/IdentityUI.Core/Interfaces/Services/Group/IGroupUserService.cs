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

        Task<Result> ValidateGroup(string groupId);
        Task<Result> ValidateUser(string id);
        Task<Result> RoleIsValid(string roleId);
        Task<Result> CanAssigneRole(string roleId);

        /// <summary>
        /// This method validates all parameters
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<Result<GroupUserEntity>> AddUserToGroupWithValidation(string userId, string groupId, string roleId);
        /// <summary>
        /// This method requires that you already validated all parameters
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<Result<GroupUserEntity>> AddUserToGroupWithoutValidation(string userId, string groupId, string roleId);
    }
}
