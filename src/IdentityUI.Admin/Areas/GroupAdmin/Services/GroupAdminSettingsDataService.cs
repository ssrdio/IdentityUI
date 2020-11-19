using Microsoft.Extensions.Logging;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Settings;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Interfaces;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Services
{
    public class GroupAdminSettingsDataService : IGroupAdminSettingsDataService
    {
        private readonly IGroupStore _groupStore;

        private readonly ILogger<GroupAdminSettingsDetailsModel> _logger;

        public GroupAdminSettingsDataService(IGroupStore groupStore, ILogger<GroupAdminSettingsDetailsModel> logger)
        {
            _groupStore = groupStore;
            _logger = logger;
        }

        public Task<Result<GroupAdminSettingsDetailsModel>> Get(string groupId)
        {
            IBaseSpecification<GroupEntity, GroupAdminSettingsDetailsModel> specification = SpecificationBuilder
                .Create<GroupEntity>()
                .Where(x => x.Id == groupId)
                .Select(x => new GroupAdminSettingsDetailsModel(
                    x.Name))
                .Build();

            return _groupStore.SingleOrDefault(specification);
        }

        public Task<Result<GroupAdminSettingsViewModel>> GetViewModel(string groupId)
        {
            GroupAdminSettingsViewModel viewModel = new GroupAdminSettingsViewModel(
                groupId: groupId);

            return Task.FromResult(Result.Ok(viewModel));
        }
    }
}
