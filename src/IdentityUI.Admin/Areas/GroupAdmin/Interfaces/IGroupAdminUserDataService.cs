using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.User;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces
{
    public interface IGroupAdminUserDataService
    {
        Task<Result<GroupAdminUserIndexViewModel>> GetIndexViewModel();
        Task<Result<GroupAdminUserDetailsViewModel>> GetDetailsViewModel(long groupUserId);

        Task<Result<GroupAdminUserDetailsModel>> Get(long groupUserId);
    }
}
