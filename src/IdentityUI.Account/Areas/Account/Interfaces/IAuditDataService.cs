using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Account.Areas.Account.Models.Audit;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Interfaces
{
    public interface IAuditDataService
    {
        Task<Result<DataTableResult<AuditTableModel>>> Get(DataTableRequest dataTableRequest, AuditTableRequest auditTableRequest);
        Task<Result<AuditDetailsModel>> Get(long id);

        AuditIndexViewModel GetIndexViewModel();
    }
}
