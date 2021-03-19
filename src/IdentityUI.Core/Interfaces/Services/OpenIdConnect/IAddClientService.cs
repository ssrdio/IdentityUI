using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Services.OpenIdConnect.Models;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.OpenIdConnect
{
    public interface IAddClientService
    {
        Task<Result<IdStringModel>> AddSinglePageClient(AddSinglePageClientModel addSinglePageClientModel);
        Task<Result<IdStringModel>> AddWebAppClient(AddWebAppClientModel addServerSideClient);
        Task<Result<IdStringModel>> AddClientCredentials(AddClientCredentialsClientModel addClientCredentialsClientModel);
        Task<Result<IdStringModel>> AddMobileClient(AddMobileClientModel addMobileClient);
        Task<Result<IdStringModel>> AddCustomClient(AddCustomClientModel addCustomClient);
    }
}
