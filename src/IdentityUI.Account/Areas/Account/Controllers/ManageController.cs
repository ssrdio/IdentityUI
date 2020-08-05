using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSRD.IdentityUI.Account.Areas.Account.Models;
using SSRD.IdentityUI.Account.Areas.Account.Models.Manage;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.Credentials.Models;
using SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth.Models;
using SSRD.IdentityUI.Core.Services.User.Models;
using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using Microsoft.AspNetCore.Http;

namespace SSRD.IdentityUI.Account.Areas.Account.Controllers
{
    public class ManageController : BaseController
    {
        private readonly ITwoFactorAuthService _twoFactorAuthService;
        private readonly IManageDataService _manageDataService;
        private readonly IManageUserService _manageUserService;
        private readonly ICredentialsService _credentialsService;

        public ManageController(ITwoFactorAuthService twoFactorAuthService, IManageDataService manageDataService, IManageUserService manageUserService,
            ICredentialsService credentialsService)
        {
            _twoFactorAuthService = twoFactorAuthService;
            _manageDataService = manageDataService;
            _manageUserService = manageUserService;
            _credentialsService = credentialsService;
        }

        [HttpGet("/[area]/[controller]")]
        [HttpGet("/[area]/[controller]/[action]")]
        public IActionResult Profile()
        {
            Result<ProfileViewModel> result = _manageDataService.GetProfile(GetUserId());
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

            Result editResult = _manageUserService.EditUser(GetUserId(), request);

            Result<ProfileViewModel> getResult = _manageDataService.GetProfile(GetUserId());
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

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Profile));
            }

            Result result = await _credentialsService.ChangePassword(GetUserId(), GetSessionCode(), GetIp(), request);
            ChangePasswordViewModel viewModel;

            if (result.Failure)
            {
                viewModel = new ChangePasswordViewModel(StatusAlertViewExtension.Get(result));

                ModelState.AddErrors(result.Errors);
                return View(viewModel);
            }
            viewModel = new ChangePasswordViewModel(StatusAlertViewExtension.Get("Password updated"));

            return View(viewModel);
        }

        [HttpPost("/[area]/[controller]/[action]")]
        public IActionResult ProfileImage(IFormFile files)
        {
            if (files != null)
            {
                if (files.Length > 0)
                {
                    var fileName = System.IO.Path.GetFileName(files.FileName);
                    var fileExtension = System.IO.Path.GetExtension(fileName);
                    var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);

                    using (var target = new System.IO.MemoryStream())
                    {
                        files.CopyTo(target);

                        Result result = _manageUserService.UpdateProfileImage(GetUserId(),
                                                                              target.ToArray(),
                                                                              newFileName);

                        if (result.Failure)
                        {
                            ModelState.AddErrors(result.Errors);
                            return RedirectToAction(nameof(Profile));
                        }
                    }

                    Result<ProfileViewModel> profileResult = _manageDataService.GetProfile(GetUserId());
                    profileResult.Value.StatusAlert = StatusAlertViewExtension.Get("Profile Image updated");

                    return View(nameof(Profile), profileResult.Value);
                }
            }
            return RedirectToAction(nameof(Profile));
        }
    }
}
