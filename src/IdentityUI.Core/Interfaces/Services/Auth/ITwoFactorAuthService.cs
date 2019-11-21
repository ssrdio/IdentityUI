using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Auth
{
    public interface ITwoFactorAuthService
    {
        Task<Result<(string sharedKey, string authenticatorUri)>> Generate2faCode(string userId, string sessionCode, string ip);
        Task<Result> VerifyTwoFactorCode(string userId, string sessionCode, string ip, AddTwoFactorAuthenticatorRequest request);

        Task<Result> Disable(string userId);
        Task<Result> Reset(string userId, string sessionCode, string ip);
    }
}
