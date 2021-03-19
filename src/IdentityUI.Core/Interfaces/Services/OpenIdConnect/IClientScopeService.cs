using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect;
using SSRD.IdentityUI.Core.Services.OpenIdConnect.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.OpenIdConnect
{
    public interface IClientScopeService
    {
        Task<Result> Add(AddClientScopeModel addClientScopeModel);
        Task<Result<ClientScopeEntity>> Update(string id, UpdateClientScopeModel updateClientScopeModel);
        Task<Result> Remove(string id);
    }
}
