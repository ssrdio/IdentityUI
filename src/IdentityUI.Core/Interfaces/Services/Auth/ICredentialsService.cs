using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.Credentials.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Auth
{
    public interface ICredentialsService
    {
        Task<Result> RecoverPassword(RecoverPasswordRequest request);
        Task<Result> ResetPassword(ResetPasswordRequest request);

        Task<Result> ChangePassword(string userdId, string sessionCode, string ip, ChangePasswordRequest request);
    }
}
