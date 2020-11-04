using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.User;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces
{
    public interface IGroupAdminUserDataService
    {
        Task<Result<GroupAdminUserIndexViewModel>> GetIndexViewModel(string groupId);
        Task<Result<GroupAdminUserDetailsViewModel>> GetDetailsViewModel(string groupId, long groupUserId);

        Task<Result<GroupAdminUserDetailsModel>> Get(long groupUserId);
    }
}
