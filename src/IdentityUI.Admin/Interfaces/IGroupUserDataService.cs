using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Models.Group;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Interfaces
{
    public interface IGroupUserDataService
    {
        Task<Result<DataTableResult<GroupUserTableModel>>> Get(string groupId, DataTableRequest request);

        Task<Result<Select2Result<Select2ItemBase>>> GetAvailableUsers(Select2Request select2Request);
    }
}
