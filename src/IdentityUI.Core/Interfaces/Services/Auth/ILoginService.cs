using Microsoft.AspNetCore.Identity;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.Login.Models;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Auth
{
    public interface ILoginService
    {
        Task<SignInResult> Login(string ip, string sessionCode, LoginRequest loginRequest);
        /// <summary>
        /// Used to login user after password change, 2fa change
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionCode"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        Task<Result> Login(string userId, string sessionCode, string ip);

        /// <summary>
        /// Used to login user after password change, 2fa change
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result> Login(string userId);
        Task<SignInResult> LoginWith2fa(string ip, LoginWith2faRequest loginWith2FaRequest);

        Task<Result> LoginWithRecoveryCode(LoginWithRecoveryCodeRequest loginWithRecoveryCode);

        Task<Result> Logout(string userId, string sessionCode);
    }
}