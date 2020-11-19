using SSRD.CommonUtils.Result;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Auth
{
    public interface IImpersonateService
    {
        Task<Result> Start(string userId);
        Task<Result> Start(long groupUserId);

        Task<Result> Stop();
    }
}
