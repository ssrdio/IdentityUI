using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Attributes;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Settings;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Services.Group.Models;
using SSRD.IdentityUI.Core.Services.Identity;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers.Api
{
    [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_MANAGE_SETTINGS)]
    public class SettingsController : GroupAdminApiBaseController
    {
        private readonly IGroupAdminSettingsDataService _groupAdminSettingsDataService;
        private readonly IGroupService _groupService;

        private readonly IdentityUIEndpoints _identityUIEndpoints;

        public SettingsController(
            IGroupAdminSettingsDataService groupAdminSettingsDataService,
            IGroupService groupService,
            IOptions<IdentityUIEndpoints> identityUIEndpoints)
        {
            _groupAdminSettingsDataService = groupAdminSettingsDataService;
            _groupService = groupService;
            _identityUIEndpoints = identityUIEndpoints.Value;
        }

        [HttpGet]
        [ProducesResponseType(typeof(GroupAdminSettingsDetailsModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] string groupId)
        {
            Result<GroupAdminSettingsDetailsModel> result = await _groupAdminSettingsDataService.Get(groupId);

            return result.ToApiResult();
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] string groupId, [FromBody] UpdateGroupModel updateGroupModel)
        {
            if(!_identityUIEndpoints.CanChangeGroupName)
            {
                return NotFound();
            }

            Result result = await _groupService.Update(groupId, updateGroupModel);

            return result.ToApiResult();
        }

        [HttpDelete]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] string groupId, [FromBody] UpdateGroupModel updateGroupModel)
        {
            if (!_identityUIEndpoints.CanRemoveGroup)
            {
                return NotFound();
            }

            Result result = await _groupService.RemoveAsync(groupId);

            return result.ToApiResult();
        }
    }
}
