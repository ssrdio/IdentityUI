using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Auth
{
    public interface ICanLoginService
    {
        Task<Result> CanLogin(AppUserEntity appUserEntity);
    }
}
