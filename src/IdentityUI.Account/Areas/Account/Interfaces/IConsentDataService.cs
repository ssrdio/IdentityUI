using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Account.Areas.Account.Models.Consent;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Interfaces
{
    public interface IConsentDataService
    {
        Task<Result<DataTableResult<ConsentModel>>> Get(DataTableRequest dataTableRequest);
    }
}
