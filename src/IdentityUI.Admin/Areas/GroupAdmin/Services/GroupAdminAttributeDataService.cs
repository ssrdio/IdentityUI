using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Attribute;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Services
{
    public class GroupAdminAttributeDataService : IGroupAdminAttributeDataService
    {
        public Task<Result<GroupAdminAttributeViewModel>> GetViewModel(string groupId)
        {
            GroupAdminAttributeViewModel viewModel = new GroupAdminAttributeViewModel(
                groupId: groupId);

            return Task.FromResult(Result.Ok(viewModel));
        }
    }
}
