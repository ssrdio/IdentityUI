using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Interfaces;
using SSRD.IdentityUI.Admin.Models.Group;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Services.Identity;
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

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<GroupInviteTableModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] DataTableRequest dataTableRequest)
        {
            Result<DataTableResult<GroupInviteTableModel>> result = await _groupInviteDataService.Get(User.GetGroupId(_identityUIClaimOptions), dataTableRequest);

            return result.ToApiResult();
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Add([FromBody] InviteToGroupRequest request)
        {
            Core.Models.Result.Result result = await _inviteService.InviteToGroup(User.GetGroupId(_identityUIClaimOptions), request);

            return result.ToNewResult().ToApiResult();
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public IActionResult Remove([FromRoute] string id)
        {
            //TODO: check if invite belong to user group
            Core.Models.Result.Result result = _inviteService.Remove(id);

            return result.ToNewResult().ToApiResult();
        }
    }
}
