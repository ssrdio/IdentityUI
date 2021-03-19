using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models;
using SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.OpenIdConnect
{
    public interface IClientDataService
    {
        Task<Result<ClientMenuViewModel>> GetMenuViewModel(string id);
        Task<Result<ClientDetailsViewModel>> GetDetailsViewModel(string id);
        Task<Result<ClientConsentViewModel>> GetConsentViewModel(string id);
        Task<Result<ClientTokenViewModel>> GetTokenViewModel(string id);

        Task<Result<DataTableResult<ClientTableModel>>> Get(DataTableRequest dataTableRequest);
        Task<Result<ClientDetailsModel>> Get(string id);
        Task<Result<ClientDetailsModel>> Get(ClientEntity client);

        Task<Result<ClientIdModel>> GetClientId(string id);
        Task<Result<ClientSecretModel>> GetClientSecret(string id);
        Task<Result<ClientScopesModel>> GetScopes(string id);
        Task<Result<Select2Result<Select2Item>>> GetConsentSubjects(string id, Select2Request select2Request);
        Task<Result<Select2Result<Select2Item>>> GetTokenSubjects(string id, Select2Request select2Request);

        Task<Result<DataTableResult<ClientConsentTableModel>>> GetConsents(
            string id,
            DataTableRequest dataTableRequest,
            ClientConsentTableFilterModel clientConsentTableFilterModel);
        Task<Result<DataTableResult<ClientTokenTableModel>>> GetTokens(string id, DataTableRequest dataTableRequest, ClientTokenTableFilterModel clientTokenTableFilter);
    }
}
