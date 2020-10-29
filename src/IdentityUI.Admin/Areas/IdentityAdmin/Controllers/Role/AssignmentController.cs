using System.Collections.Generic;
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

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers
{
    //TODO: change this to api controller
    [Route("[area]/Role/{roleId:required}/[controller]/[action]")]
    public class AssignmentController : BaseController
    {
        private readonly IRoleAssignmentDataService _roleAssignmentDataService;
        private readonly IRoleAssignmentService _roleAssignmentService;

        public AssignmentController(IRoleAssignmentDataService roleAssignmentDataService, IRoleAssignmentService roleAssignmentService)
        {
            _roleAssignmentDataService = roleAssignmentDataService;
            _roleAssignmentService = roleAssignmentService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<RoleAssignmentTableModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Get([FromRoute] string roleId, [FromQuery] DataTableRequest dataTableRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<DataTableResult<RoleAssignmentTableModel>> result = _roleAssignmentDataService.Get(roleId, dataTableRequest);
            if (result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return BadRequest(ModelState);
            }

            return Ok(result.Value);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Select2ItemBase>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult GetUnassigned([FromRoute] string roleId, [FromQuery] Select2Request select2Request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<Select2Result<Select2ItemBase>> result = _roleAssignmentDataService.GetUnassigned(roleId, select2Request);
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
        public IActionResult Add([FromRoute] string roleId, [FromBody] AddRoleAssignmentRequest addRoleAssignment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result result = _roleAssignmentService.Add(roleId, addRoleAssignment);
            if (result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [HttpPost("{roleAssignmentId}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Remove([FromRoute] string roleId, [FromRoute] string roleAssignmentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Result result = _roleAssignmentService.Remove(roleId, roleAssignmentId);
            if (result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }
    }
}
