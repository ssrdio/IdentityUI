using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Invite;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.User.Models;
using System.Collections.Generic;
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
        [ProducesResponseType(typeof(DataTableResult<InviteTableModel>), StatusCodes.Status200OK)]
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

        [HttpGet]
        [ProducesResponseType(typeof(Select2Result<Select2ItemBase>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult GetRoles([FromQuery] Select2Request select2Request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Result<Select2Result<Select2ItemBase>> result = _inviteDataService.GetGlobalRoles(select2Request);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(result.Value);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Select2Result<Select2ItemBase>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult GetGroups([FromQuery] Select2Request select2Request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Result<Select2Result<Select2ItemBase>> result = _inviteDataService.GetGroups(select2Request);
            if(result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(result.Value);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Select2Result<Select2ItemBase>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult GetGroupRoles([FromQuery] Select2Request select2Request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Result<Select2Result<Select2ItemBase>> result = _inviteDataService.GetGroupRoles(select2Request);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(result.Value);
        }
    }
}
