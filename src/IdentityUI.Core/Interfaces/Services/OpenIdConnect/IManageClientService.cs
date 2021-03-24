using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect;
using SSRD.IdentityUI.Core.Services.OpenIdConnect.Models;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.OpenIdConnect
{
    public interface IManageClientService
    {
        Task<Result<ClientEntity>> Update(string id, UpdateClientModel updateClientModel);
        Task<Result> Remove(string id);

        Task<Result> UpdateClientId(string id, UpdateClientIdModel updateClientIdModel);
        
        Task<Result<GenerateNewClientSecretModel>> GenerateNewClientSecret(string id, string secret = null);
        Task<Result> RemoveClientSecret(string id);

        Task<Result> AddScopes(string id, ManageClientScopesModel clientScopes);
        Task<Result> RemoveScopes(string id, ManageClientScopesModel clientScopes);
    }
}
