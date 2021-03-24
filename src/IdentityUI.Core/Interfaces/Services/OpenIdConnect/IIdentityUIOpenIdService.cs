using SSRD.CommonUtils.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.OpenIdConnect
{
    public interface IIdentityUIOpenIdService
    {
        Task<Result<Dictionary<string, object>>> GetUserInfo();
    }
}
