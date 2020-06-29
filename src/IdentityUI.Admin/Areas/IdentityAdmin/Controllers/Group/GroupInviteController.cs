using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Group;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Group;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.User.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers.Group
{
    [Route("[area]/Group/{groupId}/[controller]/[action]")]
    [GroupPermissionAuthorize(IdentityUIPermissions.GROUP_CAN_MANAGE_INVITES)]
    public class GroupInviteController : BaseController
    {
        private readonly IGroupInviteDataService _groupInviteDataService;

        private readonly IInviteService _inviteService;

        public GroupInviteController(IGroupInviteDataService groupInviteDataService, IInviteService inviteService)
        {
            _groupInviteDataService = groupInviteDataService;
            _inviteService = inviteService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<GroupInviteTableModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Get([FromRoute] string groupId, [FromQuery] DataTableRequest dataTableRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<DataTableResult<GroupInviteTableModel>> result = _groupInviteDataService.Get(groupId, dataTableRequest);
            if(result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(result.Value);
        }

        [GroupPermissionAuthorize(IdentityUIPermissions.GROUP_CAN_INVITE_USERS)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Invite([FromRoute] string groupId, [FromBody] InviteToGroupRequest inviteToGroupRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Result result = await _inviteService.InviteToGroup(groupId, inviteToGroupRequest);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [HttpPost("{inviteId}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Remove([FromRoute] string groupId, [FromRoute] string inviteId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Result result = _inviteService.Remove(inviteId);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }
    }
}
