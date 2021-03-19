using SSRD.CommonUtils.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.OpenIdConnect
{
    public interface IClientTokenService
    {
        Task<Result> RevokeTokens(List<string> tokenIds);
    }
}
