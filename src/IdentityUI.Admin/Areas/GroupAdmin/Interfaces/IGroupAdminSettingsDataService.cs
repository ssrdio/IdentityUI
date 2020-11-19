using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Settings;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces
{
    public interface IGroupAdminSettingsDataService
    {
        Task<Result<GroupAdminSettingsViewModel>> GetViewModel(string groupId);

        Task<Result<GroupAdminSettingsDetailsModel>> Get(string groupId);
    }
}
