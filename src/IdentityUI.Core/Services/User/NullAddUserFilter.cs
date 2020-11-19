using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Services.User.Models;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.User
{
    public class NullAddUserFilter : IAddUserFilter
    {
        public Task<Result> AfterAdded(AppUserEntity appUser)
        {
            return Task.FromResult(Result.Ok());
        }

        public Task<Result> BeforeAdd(BaseRegisterRequest baseRegisterRequest)
        {
            return Task.FromResult(Result.Ok());
        }
    }
}
