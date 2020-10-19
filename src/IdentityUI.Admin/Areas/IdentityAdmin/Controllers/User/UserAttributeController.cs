using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.User;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.User;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.User.Models.Attribute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers.User
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/[area]/User/{userId}/Attributes/[action]")]
    public class UserAttributeController : BaseController
    {
        private IUserAttributeService _userAttributeService;
        private IUserDataService _userDataService;
        private IUserAttributeDataService _userAttributeDataService;

        public UserAttributeController(IUserAttributeService userAttributeService, IUserDataService userDataService, IUserAttributeDataService userAttributeDataService)
        {
            _userAttributeService = userAttributeService;
            _userDataService = userDataService;
            _userAttributeDataService = userAttributeDataService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("/[area]/User/{userId}/Attributes")]
        public IActionResult Index([FromRoute] string userId)
        {
            Result<UserMenuViewModel> viewModel = _userDataService.GetUserMenuViewModel(userId);
            if(viewModel.Failure)
            {
                return NotFoundView();
            }

            return View(viewModel.Value);
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromRoute] string userId, [FromQuery] DataTableRequest dataTableRequest)
        {
            Result<DataTableResult<UserAttributeTableModel>> result = await _userAttributeDataService.Get(userId, dataTableRequest);
            if(result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(result);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromRoute] string userId, [FromBody] AddUserAttributeModel addUserAttribute)
        {
            Result result = await _userAttributeService.Add(userId, addUserAttribute);
            if(result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [HttpPost("{attributeId:long}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromRoute] string userId, [FromRoute] long attributeId, [FromBody] UpdateUserAttributeModel updateUserAttribute)
        {
            Result result = await _userAttributeService.Update(userId, attributeId, updateUserAttribute);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [HttpDelete("{attributeId:long}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Remove([FromRoute] string userId, [FromRoute] long attributeId)
        {
            Result result = await _userAttributeService.Remove(userId, attributeId);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }
    }
}
