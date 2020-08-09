using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.User.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services
{
    public interface IProfileImageService
    {
        Task<Result> UpdateProfileImage(string userId, UploadProfileImageRequest uploadProfileImageRequest);
        Task<Result<string>> GetProfileImageURL(string userId);
        Task<Result<FileData>> GetProfileImage(string userId);
    }
}
