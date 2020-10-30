using Microsoft.AspNetCore.Mvc;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.User;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Services.User.Models;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers.Mvc
{
    public class UserController : GroupAdminBaseController
    {
        private readonly IGroupAdminUserDataService _groupAdminUserDataService;
        private readonly IManageUserService _manageUserService;

        public UserController(IGroupAdminUserDataService groupAdminUserDataService, IManageUserService manageUserService)
        {
            _groupAdminUserDataService = groupAdminUserDataService;
            _manageUserService = manageUserService;
        }

        [HttpGet("/[area]/[controller]")]
        public async Task<IActionResult> Index()
        {
            Result<GroupAdminUserIndexViewModel> result = await _groupAdminUserDataService.GetIndexViewModel();
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }
        
        [HttpGet("/[area]/[controller]/[action]/{groupUserId}")]
        public async Task<IActionResult> Details([FromRoute] long groupUserId)
        {
            Result<GroupAdminUserDetailsViewModel> result = await _groupAdminUserDataService.GetDetailsViewModel(groupUserId);
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [HttpPost("/[area]/[controller]/[action]/{groupUserId}")]
        public async Task<IActionResult> Details(long groupUserId, EditUserRequest editUserRequest)
        {
            Result<GroupAdminUserDetailsViewModel> result = await _groupAdminUserDataService.GetDetailsViewModel(groupUserId);
            if (result.Failure)
            {
                return NotFoundView();
            }

            GroupAdminUserDetailsViewModel viewModel = result.Value;

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            Result editResult = await _manageUserService.EditUser(groupUserId, editUserRequest);
            if(editResult.Failure)
            {
                viewModel.StatusAlert = StatusAlertViewExtension.Get(editResult.ToOldResult());
            }
            else
            {
                viewModel.StatusAlert = StatusAlertViewExtension.Get("User updated");
            }

            return View(result.Value);
        }
    }
}
