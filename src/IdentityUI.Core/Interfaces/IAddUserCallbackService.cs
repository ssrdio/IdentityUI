using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces
{
    public interface IAddUserCallbackService
    {
        Task<Result> AfterUserAdded(AppUserEntity appUser);
    }
}
