using SSRD.CommonUtils.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.OpenIdConnect
{
    public interface IClientConsentService
    {
        Task<Result> SetValidStatus(List<string> consentIds);
        Task<Result> SetRevokedStatus(List<string> consentIds);
    }
}
