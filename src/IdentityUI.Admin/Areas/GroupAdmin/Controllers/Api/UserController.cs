﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Interfaces;
using SSRD.IdentityUI.Admin.Models.Group;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Services.Group.Models;
using SSRD.IdentityUI.Core.Services.Identity;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers.Api
{
    public class UserController : GroupAdminApiBaseController
    {
        private readonly IGroupUserDataService _groupUserDataService;
        private readonly IGroupUserService _groupUserService;
        private readonly IManageUserService _manageUserService;
        private readonly IImpersonateService _impersonateService;

        private readonly IdentityUIClaimOptions _identityUIClaimOptions;

        public UserController(
            IGroupUserDataService groupUserDataService,
            IGroupUserService groupUserService,
            IManageUserService manageUserService,
            IImpersonateService impersonateService,
            IOptions<IdentityUIClaimOptions> identityUICalimOptions)
        {
            _groupUserDataService = groupUserDataService;
            _groupUserService = groupUserService;

            _manageUserService = manageUserService;
            _impersonateService = impersonateService;

            _identityUIClaimOptions = identityUICalimOptions.Value;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<GroupUserTableModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] DataTableRequest dataTableRequest)
        {
            Result<DataTableResult<GroupUserTableModel>> result = await _groupUserDataService.Get(User.GetGroupId(_identityUIClaimOptions), dataTableRequest);

            return result.ToApiResult();
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<GroupUserTableModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailable([FromQuery] Select2Request select2Request)
        {
            Result<Select2Result<Select2ItemBase>> result = await _groupUserDataService.GetAvailableUsers(select2Request);

            return result.ToApiResult();
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public IActionResult AddExisting([FromBody] AddExistingUserRequest addExistingUserRequest)
        {
            Core.Models.Result.Result result = _groupUserService.AddExisting(User.GetGroupId(_identityUIClaimOptions), addExistingUserRequest);
            if(result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [HttpPost("{groupUserId}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public IActionResult ChangeRole([FromRoute] long groupUserId, [FromQuery] string roleId)
        {
            Core.Models.Result.Result result = _groupUserService.ChangeRole(groupUserId, roleId, User.GetUserId(_identityUIClaimOptions));
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public IActionResult Leave()
        {
            Core.Models.Result.Result result = _groupUserService.Leave(User.GetUserId(_identityUIClaimOptions), User.GetGroupId(_identityUIClaimOptions));
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [HttpPost("{groupUserId}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public IActionResult Remove([FromRoute] long groupUserId)
        {
            Core.Models.Result.Result result = _groupUserService.Remove(groupUserId);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [HttpGet("{groupUserId}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Unlock([FromRoute] long groupUserId)
        {
            Result result = await _manageUserService.UnlockUser(groupUserId);

            return result.ToApiResult();
        }

        [HttpGet("{groupUserId}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendVereficationMain([FromRoute] long groupUserId)
        {
            Result result = await _manageUserService.SendEmilVerificationMail(groupUserId);

            return result.ToApiResult();
        }

        [HttpGet("{groupUserId}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> StartImpersonation([FromRoute] long groupUserId)
        {
            Result result = await _impersonateService.Start(groupUserId);

            return result.ToApiResult();
        }

        [HttpGet]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> StopImpersonation()
        {
            Result result = await _impersonateService.Stop();

            return result.ToApiResult();
        }
    }
}