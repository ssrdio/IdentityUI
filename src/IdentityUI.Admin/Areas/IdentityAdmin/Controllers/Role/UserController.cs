using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Role;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers.Role
{
    [Route("[area]/Role/{roleId:required}/[controller]/[action]")]
    public class UserController : BaseController
    {
        private readonly IRoleDataService _roleDataService;
        private readonly IManageUserService _manageUserService;

        public UserController(IRoleDataService roleDataService, IManageUserService manageUserService)
        {
            _roleDataService = roleDataService;
            _manageUserService = manageUserService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<UserTableModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult GetGlobal([FromRoute] string roleId, [FromQuery] DataTableRequest dataTableRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<DataTableResult<UserTableModel>> result = _roleDataService.GetGlobalUsers(roleId, dataTableRequest);
            if (result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return BadRequest(ModelState);
            }

            return Ok(result.Value);
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<UserTableModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult GetGroup([FromRoute] string roleId, [FromQuery] DataTableRequest dataTableRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<DataTableResult<UserTableModel>> result = _roleDataService.GetGroupUsers(roleId, dataTableRequest);
            if (result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return BadRequest(ModelState);
            }

            return Ok(result.Value);
        }

        [HttpPost("{userId}")]
        [ProducesResponseType(typeof(DataTableResult<EmptyResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Remove([FromRoute] string roleId, [FromRoute] string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result result = await _manageUserService.RemoveRole(userId, roleId);
            if (result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }
    }
}
