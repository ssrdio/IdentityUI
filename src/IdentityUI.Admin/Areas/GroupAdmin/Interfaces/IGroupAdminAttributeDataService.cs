using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Attribute;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces
{
    public interface IGroupAdminAttributeDataService
    {
        Task<Result<GroupAdminAttributeViewModel>> GetViewModel(string groupId);
    }
}
