using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Services.User.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services
{
    public interface IManageUserService
    {
        Task<Core.Models.Result.Result> EditUser(string id, EditUserRequest editUserRequest, string adminId);
        Task<Result> EditUser(long groupUserId, EditUserRequest editUserRequest);

        Task<Core.Models.Result.Result> SetNewPassword(string userId, SetNewPasswordRequest setNewPasswordRequest, string adminId);
        Task<Core.Models.Result.Result> RemoveRoles(string userId, List<string> roles, string adminId);
        Task<Core.Models.Result.Result> AddRoles(string userId, List<string> roles, string adminId);
        Core.Models.Result.Result EditUser(string id, EditProfileRequest editProfileRequest);

        Core.Models.Result.Result UnlockUser(UnlockUserRequest request, string adminId);
        Task<Result> UnlockUser(long groupUserId);

        Task<Core.Models.Result.Result> SendEmilVerificationMail(SendEmailVerificationMailRequest request, string adminId);
        Task<Result> SendEmilVerificationMail(long groupUserId);

        Task<Core.Models.Result.Result> RemoveRole(string userId, string roleId);

        Task<Result> RemoveUser(string userId);
    }
}
