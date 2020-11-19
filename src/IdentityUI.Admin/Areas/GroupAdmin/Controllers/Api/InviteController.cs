using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Attributes;
using SSRD.IdentityUI.Admin.Interfaces;
using SSRD.IdentityUI.Admin.Models.Group;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Services.User.Models;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers.Api
{
    public class InviteController : GroupAdminApiBaseController
    {
        private readonly IGroupInviteDataService _groupInviteDataService;
        private readonly IInviteService _inviteService;
        private readonly IdentityUIClaimOptions _identityUIClaimOptions;

        public InviteController(
            IGroupInviteDataService groupInviteDataService,
            IInviteService inviteService,
            IOptions<IdentityUIClaimOptions> identityUIClaimOptions)
        {
            _groupInviteDataService = groupInviteDataService;
            _inviteService = inviteService;
            _identityUIClaimOptions = identityUIClaimOptions.Value;
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_MANAGE_INVITES)]
        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<GroupInviteTableModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] string groupId, [FromQuery] DataTableRequest dataTableRequest)
        {
            Result<DataTableResult<GroupInviteTableModel>> result = await _groupInviteDataService.Get(
                groupId,
                dataTableRequest);

            return result.ToApiResult();
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_INVITE_USERS)]
        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Add([FromRoute] string groupId, [FromBody] InviteToGroupRequest request)
        {
            Core.Models.Result.Result result = await _inviteService.InviteToGroup(groupId, request);

            return result.ToNewResult().ToApiResult();
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_MANAGE_INVITES)]
        [HttpPost("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove([FromRoute] string groupId, [FromRoute] string id)
        {
            Result result = await _inviteService.Remove(groupId, id);

            return result.ToApiResult();
        }
    }
}
