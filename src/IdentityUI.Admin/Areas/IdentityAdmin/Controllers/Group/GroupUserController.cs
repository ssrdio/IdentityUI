﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Group;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Group;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Group.Models;
using SSRD.IdentityUI.Core.Services.Identity;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers.Group
{
    //TODO: change this to api controller
    [Route("[area]/Group/{groupId}/[controller]/[action]")]
    [AllowAnonymous]
    [GrouPermissionAuthorize(IdentityUIPermissions.GROUP_CAN_SEE_USERS)]
    public class GroupUserController : BaseController
    {
        private readonly IGroupUserService _groupUserService;
        private readonly IGroupUserDataService _groupUserDataService;

        public GroupUserController(IGroupUserService groupUserService, IGroupUserDataService groupUserDataService)
        {
            _groupUserService = groupUserService;
            _groupUserDataService = groupUserDataService;
        }

        [GrouPermissionAuthorize(IdentityUIPermissions.GROUP_CAN_SEE_USERS)]
        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<GroupUserTableModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Get([FromRoute]string groupId, [FromQuery] DataTableRequest dataTableRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<DataTableResult<GroupUserTableModel>> result = _groupUserDataService.Get(groupId, dataTableRequest);
            if (result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return BadRequest(ModelState);
            }

            return Ok(result.Value);
        }

        [GrouPermissionAuthorize(IdentityUIPermissions.GROUP_CAN_ADD_EXISTING_USERS)]
        [HttpGet]
        [ProducesResponseType(typeof(Select2Result<Select2ItemBase>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult GetAvailable([FromQuery] Select2Request select2Request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<Select2Result<Select2ItemBase>> result = _groupUserDataService.GetAvailable(select2Request);
            if (result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return BadRequest(ModelState);
            }

            return Ok(result.Value);
        }

        [GrouPermissionAuthorize(IdentityUIPermissions.GROUP_CAN_ADD_EXISTING_USERS)]
        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult AddExisting([FromRoute] string groupId, [FromBody] AddExistingUserRequest addExistingUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result result = _groupUserService.AddExisting(groupId, addExistingUser);
            if(result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [GrouPermissionAuthorize(IdentityUIPermissions.GROUP_CAN_INVITE_USERS)]
        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Invite([FromRoute] string groupId, [FromBody] InviteUserToGroupRequest inviteUserToGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result result = await _groupUserService.Invite(groupId, inviteUserToGroup);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [GrouPermissionAuthorize(IdentityUIPermissions.GROUP_CAN_REMOVE_USERS)]
        [HttpPost("{groupUserId}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Remove([FromRoute] string groupId, [FromRoute] long groupUserId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result result = _groupUserService.Remove(groupUserId, User.GetUserId(), User.GetGroupId(), User.HasPermission(IdentityUIPermissions.GROUP_CAN_REMOVE_USERS));
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [GrouPermissionAuthorize(IdentityUIPermissions.GROUP_CAN_MANAGE_ROLES)]
        [HttpPost("{groupUserId}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult ChangeRole([FromRoute] string groupId, [FromRoute] long groupUserId, [FromQuery] string roleId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result result = _groupUserService.ChangeRole(groupUserId, roleId, User.GetUserId(), User.GetGroupId(),
                User.HasPermission(IdentityUIPermissions.GROUP_CAN_MANAGE_ROLES));
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
        public IActionResult Leave([FromRoute] string groupId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result result = _groupUserService.Leave(User.GetUserId(), groupId);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }
    }
}