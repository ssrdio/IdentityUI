using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Attributes;
using SSRD.IdentityUI.Admin.Interfaces;
using SSRD.IdentityUI.Admin.Models.Group;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;
using SSRD.IdentityUI.Core.Services.Group.Models;
using SSRD.IdentityUI.Core.Services.Identity;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers.Api
{
    [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_MANAGE_ATTRIBUTES)]
    public class AttributeController : GroupAdminApiBaseController
    {
        private readonly IGroupAttributeDataService _groupAttributeDataService;
        private readonly IGroupAttributeService _groupAttributeService;

        public AttributeController(IGroupAttributeDataService groupAttributeDataService, IGroupAttributeService groupAttributeService)
        {
            _groupAttributeDataService = groupAttributeDataService;
            _groupAttributeService = groupAttributeService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<GroupAttributeTableModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] string groupId, [FromQuery] DataTableRequest request)
        {
            Result<DataTableResult<GroupAttributeTableModel>> result = await _groupAttributeDataService.Get(groupId, request);

            return result.ToApiResult();
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public Task<IActionResult> Add([FromRoute] string groupId, [FromBody] AddGroupAttributeRequest request)
        {
            Core.Models.Result.Result result = _groupAttributeService.Add(groupId, request);

            return Task.FromResult(result.ToNewResult().ToApiResult());
        }

        [HttpPost("{groupAttributeId}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public Task<IActionResult> Update([FromRoute] string groupId, [FromRoute] long groupAttributeId, [FromBody] EditGroupAttributeRequest request)
        {
            Core.Models.Result.Result result = _groupAttributeService.Edit(groupId, groupAttributeId, request);

            return Task.FromResult(result.ToNewResult().ToApiResult());
        }

        [HttpDelete("{groupAttributeId}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public Task<IActionResult> Delete([FromRoute] string groupId, [FromRoute] long groupAttributeId)
        {
            Core.Models.Result.Result result = _groupAttributeService.Remove(groupId, groupAttributeId);

            return Task.FromResult(result.ToNewResult().ToApiResult());
        }
    }
}
