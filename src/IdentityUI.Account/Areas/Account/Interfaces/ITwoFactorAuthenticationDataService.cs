using SSRD.IdentityUI.Account.Areas.Account.Models.Manage;
using SSRD.IdentityUI.Core.Models.Result;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Interfaces
{
    public interface ITwoFactorAuthenticationDataService
    {
        Task<Result<AddPhoneTwoFactorAuthenticationViewModel>> GetPhoneViewModel();
        Task<Result<AddEmailTwoFactorAuthenticationViewModel>> GetEmailViewModel();
        Task<Result<AddTwoFactorAuthenticatorViewModel>> GetAuthenticatorViewModel();

        Result<TwoFactorAuthenticatorViewModel> GetTwoFactorAuthenticatorViewModel();
    }
}
