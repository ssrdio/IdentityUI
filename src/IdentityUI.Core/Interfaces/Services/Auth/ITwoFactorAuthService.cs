using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Auth
{
    public interface ITwoFactorAuthService
    {
        Task<Result<(string sharedKey, string authenticatorUri)>> Generate2faCode(string userId);
        Task<Result> GenerateSmsCode(string userId);
        Task<Result> GenerateAndSendEmailCode(string userId);

        Task<Result<IEnumerable<string>>> VerifyTwoFactorCode(string userId, AddTwoFactorAuthenticatorRequest request);
        Task<Result<IEnumerable<string>>> VerifyPhoneTwoFactorCode(string userId,  AddTwoFactorPhoneAuthenticationRequest request);
        Task<Result<IEnumerable<string>>> VerifyEmailTwoFactorCode(string userId, AddTwoFactorEmailAuthenticationRequest request);

        Task<Result> TrySend2faCode(string userId);

        Task<Result> Disable(string userId);
    }
}
