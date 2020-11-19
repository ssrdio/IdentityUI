using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Role;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services.Role;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Role.Models;
using System.Collections.Generic;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers.Group
{
    //TODO: change this to api controller
    [Route("[area]/Role/{roleId:required}/[controller]/[action]")]
    public class PermissionController : BaseController
    {
        private readonly IRolePermissionsDataService _groupRolePermissionsDataService;
        private readonly IRolePermissionService _groupRolePermissionService;

        public PermissionController(IRolePermissionsDataService groupRolePermissionsDataService, IRolePermissionService groupRolePermissionService)
        {
            _groupRolePermissionsDataService = groupRolePermissionsDataService;
            _groupRolePermissionService = groupRolePermissionService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<RolePermissionTableModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Get([FromRoute] string roleId, [FromQuery] DataTableRequest dataTableRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<DataTableResult<RolePermissionTableModel>> result = _groupRolePermissionsDataService.Get(roleId, dataTableRequest);
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
        public IActionResult GetAvailable([FromRoute] string roleId, [FromQuery] Select2Request select2Request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<Select2Result<Select2ItemBase>> result = _groupRolePermissionsDataService.GetAvailable(roleId, select2Request);
            if(result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Add([FromRoute] string roleId, [FromBody] AddRolePermissionRequest addGroupRolePermission)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result result = _groupRolePermissionService.Add(roleId, addGroupRolePermission);
            if(result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [HttpPost("{permissionId:required}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Remove([FromRoute] string roleId, [FromRoute] string permissionId)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result result = _groupRolePermissionService.Remove(roleId, permissionId);
            if(result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

    }
}
