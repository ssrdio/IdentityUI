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
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Group.Models;
using SSRD.IdentityUI.Core.Services.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers
{
    public class GroupController : BaseController
    {
        private readonly IGroupDataService _groupDataService;
        private readonly IGroupService _groupService;

        public GroupController(IGroupDataService groupDataService, IGroupService groupService)
        {
            _groupDataService = groupDataService;
            _groupService = groupService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.HasRole(IdentityUIRoles.IDENTITY_MANAGMENT_ROLE))
            {
                return View();
            }

            return RedirectToAction(nameof(User), new { id = User.GetGroupId() });
        }

        [AllowAnonymous]
        [GrouPermissionAuthorize(IdentityUIPermissions.GROUP_CAN_SEE_USERS)]
        [HttpGet("/[area]/[controller]/[action]/{groupId}")]

        public IActionResult Users(string groupId)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            Result<GroupUserViewModel> result = _groupDataService.GetGroupUserViewModel(groupId, User.GetUserId(),
                User.HasPermission(IdentityUIPermissions.GROUP_CAN_SEE_USERS));
            if(result.Failure)
            {
                return NotFound();
            }

            return View(result.Value);
        }

        [AllowAnonymous]
        [GrouPermissionAuthorize(IdentityUIPermissions.GROUP_CAN_MANAGE_ATTRIBUTES)]
        [HttpGet("/[area]/[controller]/[action]/{groupId}")]
        public IActionResult Attributes(string groupId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            Result<GroupMenuViewModel> result = _groupDataService.GetMenuViewModel(groupId);
            if (result.Failure)
            {
                return NotFound();
            }

            return View(result.Value);
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<GroupTableModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Get([FromQuery] DataTableRequest dataTableRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<DataTableResult<GroupTableModel>> result = _groupDataService.Get(dataTableRequest);
            if(result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return BadRequest(ModelState);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Add([FromBody] AddGroupRequest addGroup)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result result = _groupService.Add(addGroup);
            if(result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }
    }
}
