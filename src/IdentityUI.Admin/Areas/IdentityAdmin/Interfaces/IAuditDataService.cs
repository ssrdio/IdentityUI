using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.Audit.Data;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Audit;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces
{
    public interface IAuditDataService
    {
        Task<Result<DataTableResult<AuditAdminTableModel>>> Get(DataTableRequest dataTableRequest, AuditTableRequest auditTableRequest);
        Task<Result<AuditAdminDetailsModel>> Get(long id);
        AuditIndexViewModel GetIndexViewModel();

        Task<Result<Select2Result<Select2Item>>> GetObjectTypes(Select2Request select2Request);
        Task<Result<Select2Result<Select2Item>>> GetObjectIdentifiers(Select2Request select2Request, string objectType);
        Task<Result<Select2Result<Select2Item>>> GetSubjectIdentifiers(Select2Request select2Request, SubjectTypes? subjectType);
        Task<Result<Select2Result<Select2Item>>> GetResourceNames(Select2Request select2Request);
    }
}
