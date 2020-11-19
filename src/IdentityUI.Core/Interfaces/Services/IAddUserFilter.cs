using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Services.User.Models;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services
{
    public interface IAddUserFilter
    {
        Task<Result> BeforeAdd(BaseRegisterRequest baseRegisterRequest);
        Task<Result> AfterAdded(AppUserEntity appUser);
    }
}
