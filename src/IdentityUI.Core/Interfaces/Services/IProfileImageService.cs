using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Services.User.Models;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services
{
    public interface IProfileImageService
    {
        Task<Models.Result.Result> UpdateProfileImage(string userId, UploadProfileImageRequest uploadProfileImageRequest);
        Task<Models.Result.Result<string>> GetProfileImageURL(string userId);
        Task<Models.Result.Result<FileData>> GetProfileImage(string userId);

        Task<Result> Remove(string userId);

    }
}
