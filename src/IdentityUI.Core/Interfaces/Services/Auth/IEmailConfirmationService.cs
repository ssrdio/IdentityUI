using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Models.Result;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Auth
{
    public interface IEmailConfirmationService
    {
        Task<Result> ConfirmEmail(string userId, string code);
        Task<Result> SendVerificationMail(AppUserEntity appUser, string code);
    }
}
