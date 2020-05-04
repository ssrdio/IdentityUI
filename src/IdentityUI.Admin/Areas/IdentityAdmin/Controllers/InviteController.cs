using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Invite;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers
{
    public class InviteController : BaseController
    {
        private readonly IInviteService _inviteService;
        private readonly IInviteDataService _inviteDataService;

        public InviteController(IInviteService inviteService, IInviteDataService inviteDataService)
        {
            _inviteService = inviteService;
            _inviteDataService = inviteDataService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Get([FromQuery] DataTableRequest dataTableRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Result<DataTableResult<InviteTableModel>> result = _inviteDataService.Get(dataTableRequest);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] InviteRequest inviteRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Result result = await _inviteService.Invite(inviteRequest);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [HttpPost("/[area]/[controller]/[action]/{groupId}")]
        [AllowAnonymous]
        [GrouPermissionAuthorize(IdentityUIPermissions.GROUP_CAN_INVITE_USERS)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddToGroup([FromRoute] string groupId, [FromBody] InviteRequest inviteRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Result result = await _inviteService.Invite(inviteRequest);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Remove([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Result result = _inviteService.Remove(id);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }
    }
}
