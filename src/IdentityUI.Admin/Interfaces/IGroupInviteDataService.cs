using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Invite;
using SSRD.IdentityUI.Admin.Models.Group;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Interfaces
{
    public interface IGroupInviteDataService
    {
        Task<Result<DataTableResult<GroupInviteTableModel>>> Get(string groupId, DataTableRequest dataTableRequest);
    }
}
