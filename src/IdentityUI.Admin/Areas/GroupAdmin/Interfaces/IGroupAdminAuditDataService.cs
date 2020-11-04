using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.Audit.Data;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Audit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces
{
    public interface IGroupAdminAuditDataService
    {
        Task<Result<AuditIndexViewModel>> GetIndexViewModel(string groupId);
        Task<Result<Select2Result<Select2Item>>> GetObjectTypes(string groupId, Select2Request select2Request);
        Task<Result<Select2Result<Select2Item>>> GetObjectIdentifiers(string groupId, Select2Request select2Request, string objectType);
        Task<Result<Select2Result<Select2Item>>> GetSubjectIdentifiers(string groupId, Select2Request select2Request, SubjectTypes? subjectType);
        Task<Result<Select2Result<Select2Item>>> GetResourceNames(string groupId, Select2Request select2Request);
        Task<Result<DataTableResult<GroupAdminAuditTableModel>>> Get(
            string groupId,
            DataTableRequest dataTableRequest,
            GroupAdminAuditTableRequest auditTableRequest);
        Task<Result<GroupAdminAuditDetailsModel>> Get(string groupId, long id);
    }
}
