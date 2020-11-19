using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Auth
{
    public interface ILoginFilter
    {
        Task<Result> BeforeAdd(AppUserEntity appUserEntity);
        Task<Result> AfterAdded(AppUserEntity appUserEntity);
    }
}
