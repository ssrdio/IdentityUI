using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Models;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces
{
    public interface IDefaultProfileImageService
    {
        Task<Result<FileData>> Get();
    }
}
