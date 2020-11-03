using System.Threading.Tasks;
using SSRD.IdentityUI.Account.Areas.Account.Models;
using SSRD.IdentityUI.Account.Areas.Account.Models.Manage;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Services.User.Models;
using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.Audit.Attributes;
using SSRD.CommonUtils.Result;

namespace SSRD.IdentityUI.Account.Areas.Account.Controllers
{
    public class ManageController : BaseController
    {
        private readonly ITwoFactorAuthService _twoFactorAuthService;
        private readonly IManageDataService _manageDataService;
        private readonly IManageUserService _manageUserService;
        private readonly IProfileImageService _profileImageService;

        public ManageController(ITwoFactorAuthService twoFactorAuthService, IManageDataService manageDataService, IManageUserService manageUserService,
            IProfileImageService profileImageService)
        {
            _twoFactorAuthService = twoFactorAuthService;
            _manageDataService = manageDataService;
            _manageUserService = manageUserService;

            _profileImageService = profileImageService;
        }

        [HttpGet("/[area]/[controller]")]
        [HttpGet("/[area]/[controller]/[action]")]
        public IActionResult Profile()
        {
            Core.Models.Result.Result<ProfileViewModel> result = _manageDataService.GetProfile(GetUserId());
            if (result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return View("Profile");
            }

            return View("Profile", result.Value);
        }

        [HttpPost]
        public IActionResult Profile(EditProfileRequest request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Profile));
            }

            Core.Models.Result.Result editResult = _manageUserService.EditUser(GetUserId(), request);

            Core.Models.Result.Result<ProfileViewModel> getResult = _manageDataService.GetProfile(GetUserId());
            if (getResult.Failure)
            {
                ModelState.AddErrors(getResult.Errors);
                return View();
            }

            if (editResult.Failure)
            {
                getResult.Value.StatusAlert = StatusAlertViewExtension.Get(editResult);
                ModelState.AddErrors(editResult.Errors);
                return View();
            }

            getResult.Value.StatusAlert = StatusAlertViewExtension.Get("Profile updated");

            return View(getResult.Value);
        }

        [HttpPost("/[area]/[controller]/[action]")]
        public async Task<IActionResult> ProfileImage([FromForm] UploadProfileImageRequest uploadImageRequest)
        {
            Core.Models.Result.Result result = await _profileImageService.UpdateProfileImage(GetUserId(), uploadImageRequest);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [HttpGet]
        [Produces("application/octet-stream")]
        [AuditIgnore]
        public async Task<IActionResult> GetProfileImage()
        {
            Core.Models.Result.Result<FileData> result = await _profileImageService.GetProfileImage(GetUserId());
            if (result.Failure)
            {
                return NotFound();
            }

            //TODO: make this nicer
            string contentType = "application/octet-stream";
            if (result.Value.FileName.EndsWith(".svg"))
            {
                contentType = "image/svg+xml";
            }

            return File(result.Value.File, contentType, result.Value.FileName);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            Result result = await _manageUserService.RemoveUser(GetUserId());

            return result.ToApiResult();
        }
    }
}
