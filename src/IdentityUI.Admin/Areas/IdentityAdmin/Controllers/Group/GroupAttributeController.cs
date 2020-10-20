using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Attributes;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Group;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Group;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Group.Models;
using System.Collections.Generic;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers.Group
{
    //TODO: change this to api controller
    [GroupPermissionAuthorize(IdentityUIPermissions.GROUP_CAN_MANAGE_ATTRIBUTES)]
    [Route("[area]/Group/{groupId}/[controller]/[action]")]
    public class GroupAttributeController : GroupBaseController
    {
        private readonly IGroupAttributeDataService _groupAttributeDataService;
        private readonly IGroupAttributeService _groupAttributeService;

        public GroupAttributeController(IGroupAttributeDataService groupAttributeDataService, IGroupAttributeService groupAttributeService)
        {
            _groupAttributeDataService = groupAttributeDataService;
            _groupAttributeService = groupAttributeService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<GroupAttributeTableModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Get([FromRoute] string groupId, [FromQuery] DataTableRequest dataTableRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<DataTableResult<GroupAttributeTableModel>> result = _groupAttributeDataService.Get(groupId, dataTableRequest);
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
        public IActionResult Add([FromRoute] string groupId, [FromBody] AddGroupAttributeRequest addGroupAttribute)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Result result = _groupAttributeService.Add(groupId, addGroupAttribute);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Edit([FromRoute] string groupId, [FromRoute] long id, [FromBody] EditGroupAttributeRequest editGroupAttribute)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Result result = _groupAttributeService.Edit(groupId, id, editGroupAttribute);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Remove([FromRoute] string groupId, [FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Result result = _groupAttributeService.Remove(groupId, id);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }
    }
}