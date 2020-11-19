using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Attributes;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.User;
using SSRD.IdentityUI.Admin.Interfaces;
using SSRD.IdentityUI.Admin.Models.Group;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Services.Group.Models;
using SSRD.IdentityUI.Core.Services.Identity;
using SSRD.IdentityUI.Core.Services.User.Models;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers.Api
{
    public class UserController : GroupAdminApiBaseController
    {
        private readonly IGroupUserDataService _groupUserDataService;
        private readonly IGroupUserService _groupUserService;
        private readonly IManageUserService _manageUserService;
        private readonly IImpersonateService _impersonateService;

        private readonly IGroupAdminUserDataService _groupAdminUserDataService;

        private readonly IdentityUIClaimOptions _identityUIClaimOptions;
        private readonly IdentityUIEndpoints _identityUIEndpoints;

        public UserController(
            IGroupUserDataService groupUserDataService,
            IGroupUserService groupUserService,
            IManageUserService manageUserService,
            IImpersonateService impersonateService,
            IGroupAdminUserDataService groupAdminUserDataService,
            IOptions<IdentityUIClaimOptions> identityUICalimOptions,
            IOptions<IdentityUIEndpoints> identityUIEndpoints)
        {
            _groupUserDataService = groupUserDataService;
            _groupUserService = groupUserService;

            _manageUserService = manageUserService;
            _impersonateService = impersonateService;

            _groupAdminUserDataService = groupAdminUserDataService;

            _identityUIClaimOptions = identityUICalimOptions.Value;
            _identityUIEndpoints = identityUIEndpoints.Value;
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_SEE_USERS)]
        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<GroupUserTableModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] string groupId, [FromQuery] DataTableRequest dataTableRequest)
        {
            Result<DataTableResult<GroupUserTableModel>> result = await _groupUserDataService.Get(
                groupId,
                dataTableRequest);

            return result.ToApiResult();
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_ADD_EXISTING_USERS)]
        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<GroupUserTableModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailable([FromRoute] string groupId, [FromQuery] Select2Request select2Request)
        {
            Result<Select2Result<Select2ItemBase>> result = await _groupUserDataService.GetAvailableUsers(select2Request);

            return result.ToApiResult();
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_ADD_EXISTING_USERS)]
        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public IActionResult AddExisting([FromRoute] string groupId, [FromBody] AddExistingUserRequest addExistingUserRequest)
        {
            Core.Models.Result.Result result = _groupUserService.AddExisting(groupId, addExistingUserRequest);
            if(result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_MANAGE_ROLES)]
        [HttpPost("{groupUserId}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public IActionResult ChangeRole([FromRoute] string groupId, [FromRoute] long groupUserId, [FromQuery] string roleId)
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
        public IActionResult Leave([FromRoute] string groupId)
        {
            //TODO: change for impersonation

            Core.Models.Result.Result result = _groupUserService.Leave(User.GetUserId(_identityUIClaimOptions), groupId);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_REMOVE_USERS)]
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

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_MANAGE_USER_DETAILS)]
        [HttpGet("{groupUserId}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Unlock([FromRoute] long groupUserId)
        {
            Result result = await _manageUserService.UnlockUser(groupUserId);

            return result.ToApiResult();
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_MANAGE_USER_DETAILS)]
        [HttpGet("{groupUserId}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendVereficationMain([FromRoute] long groupUserId)
        {
            Result result = await _manageUserService.SendEmilVerificationMail(groupUserId);

            return result.ToApiResult();
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_IMPERSONATE_USER)]
        [HttpGet("{groupUserId}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> StartImpersonation([FromRoute] long groupUserId)
        {
            if(!_identityUIEndpoints.AllowImpersonation)
            {
                return NotFound();
            }

            Result result = await _impersonateService.Start(groupUserId);

            return result.ToApiResult();
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_IMPERSONATE_USER)]
        [HttpGet]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> StopImpersonation()
        {
            if (!_identityUIEndpoints.AllowImpersonation)
            {
                return NotFound();
            }

            Result result = await _impersonateService.Stop();

            return result.ToApiResult();
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_ACCESS_USER_DETAILS)]
        [HttpGet("{groupUserId}")]
        [ProducesResponseType(typeof(GroupAdminUserDetailsModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] long groupUserId)
        {
            Result<GroupAdminUserDetailsModel> result = await _groupAdminUserDataService.Get(groupUserId);

            return result.ToApiResult();
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_MANAGE_USER_DETAILS)]
        [HttpPost("{groupUserId}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] long groupUserId,[FromBody] EditUserRequest editUserRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result editResult = await _manageUserService.EditUser(groupUserId, editUserRequest);

            return editResult.ToApiResult();
        }
    }
}
