using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.User.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services
{
    public interface IManageUserService
    {
        Task<Result> EditUser(string id, EditUserRequest editUserRequest, string adminId);
        Task<Result> SetNewPassword(string userId, SetNewPasswordRequest setNewPasswordRequest, string adminId);
        Task<Result> RemoveRoles(string userId, List<string> roles, string adminId);
        Task<Result> AddRoles(string userId, List<string> roles, string adminId);
        Result EditUser(string id, EditProfileRequest editProfileRequest);

        Result UpdateProfileImage(string userId, byte[] image, string fileName);
        Result<string> GetProfileImageURL(string userId);

        Result UnlockUser(UnlockUserRequest request, string adminId);

        Task<Result> SendEmilVerificationMail(SendEmailVerificationMailRequest request, string adminId);

        Task<Result> RemoveRole(string userId, string roleId);
    }
}
