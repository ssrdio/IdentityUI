using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using SSRD.IdentityUI.Core.Services.Auth.Session.Models;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Auth
{
    public interface ISessionService
    {
        Models.Result.Result Add(string code, string userId, string ip);
        Task<Result> Add(string code, string userId);
        bool Validate(string sessionCode, string userId, string ip);

        Models.Result.Result Logout(LogoutSessionRequest request, string adminId);
        Models.Result.Result Logout(string code, string userId, SessionEndTypes endType);

        Task<Models.Result.Result> LogoutUser(LogoutUserSessionsRequest request, string adminId);
    }
}
