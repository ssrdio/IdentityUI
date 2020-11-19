using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Invite;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces
{
    public interface IGroupAdminInviteDataService
    {
        Task<Result<GroupAdminInviteViewModel>> GetInviteViewModel(string groupId);
    }
}
