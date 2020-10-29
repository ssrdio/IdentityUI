using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Interfaces;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.User
{
    public class NullAddUserCallback : IAddUserCallbackService
    {
        public Task<Result> AfterUserAdded(AppUserEntity appUser)
        {
            return Task.FromResult(Result.Ok());
        }
    }
}
