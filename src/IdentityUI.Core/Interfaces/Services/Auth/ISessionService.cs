using SSRD.IdentityUI.Core.Data.Enums.Entity;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.Session.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Auth
{
    public interface ISessionService
    {
        Result Add(string code, string userId, string ip);
        bool Validate(string sessionCode, string userId, string ip);

        Result Logout(LogoutSessionRequest request, string adminId);
        Result Logout(string code, string userId, SessionEndTypes endType);

        Task<Result> LogoutUser(LogoutUserSessionsRequest request, string adminId);
    }
}
