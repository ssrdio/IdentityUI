using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.Login.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Auth
{
    public interface IExternalLoginService
    {
        Task<Result<AuthenticationProperties>> ExternalLogin(ExternalLoginRequest externalLoginRequest, string returnUrl);
        Task<Result<SignInResult>> Callback(string remoteError);
    }
}
