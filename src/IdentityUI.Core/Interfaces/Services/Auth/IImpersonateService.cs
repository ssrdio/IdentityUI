using SSRD.CommonUtils.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Auth
{
    public interface IImpersonateService
    {
        Task<Result> Start(string userId);
        Task<Result> Stop();
    }
}
