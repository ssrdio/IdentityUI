using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSRD.IdentityUI.Account.Areas.Account.Models;
using SSRD.IdentityUI.Account.Areas.Account.Models.Manage;
using SSRD.IdentityUI.Account.Areas.Account.Services.Manage;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.Credentials.Models;
using SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth.Models;
using SSRD.IdentityUI.Core.Services.User.Models;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public IActionResult Index()
        {
            Result<ProfileViewModel> result = _manageDataService.GetProfile(GetUserId());
            if(result.Failure)
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
                return RedirectToAction(nameof(Index));
            }

            Result editResult = _manageUserService.EditUser(GetUserId(), request);
            //if(editResult.Failure)
            //{
            //    ModelState.AddErrors(editResult.Errors);
            //    return View();
            //}

            Result<ProfileViewModel> getResult = _manageDataService.GetProfile(GetUserId());
            if (getResult.Failure)
            {
                ModelState.AddErrors(getResult.Errors);
              //  getResult.Value.StatusAlert = StatusAlertViewModel.Get(editResult);
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
                return RedirectToAction(nameof(Index));
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

        [HttpGet]
        public IActionResult TwoFactorAuthenticator()
        {
            Result<TwoFactorAuthenticatorViewModel> result = _manageDataService.GetTwoFactorAuthenticatorViewModel(GetUserId());
            if(result.Failure)
            {
                result.Value.StatusAlert = StatusAlertViewExtension.Get(result);
                return View(result.Value);
            }

            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> AddTwoFactorAuthenticator()
        {
            Result<AddTwoFactorAuthenticatorViewModel> result = await _manageDataService.GetAddTwoFactorAuthenticatorViewModel(GetUserId(), GetSessionCode(), GetIp());
            if (result.Failure)
            {
                result.Value.StatusAlert = StatusAlertViewExtension.Get(result);
                return View(result.Value);
            }

            return View(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> AddTwoFactorAuthenticator(AddTwoFactorAuthenticatorRequest request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            string userId = GetUserId();

            Result verifyResult = await _twoFactorAuthService.VerifyTwoFactorCode(userId, GetSessionCode(), GetIp(), request);
            if (verifyResult.Failure)
            {
                ModelState.AddErrors(verifyResult.Errors);

                Result<AddTwoFactorAuthenticatorViewModel> codeResult = await _manageDataService.GetAddTwoFactorAuthenticatorViewModel(userId, GetSessionCode(), GetIp());
                if (codeResult.Failure)
                {
                    ModelState.AddErrors(verifyResult.Errors);
                    return View();
                }

                return View(codeResult.Value);
            }


            Result<TwoFactorAuthenticatorViewModel> result = _manageDataService.GetTwoFactorAuthenticatorViewModel(GetUserId());
            if (result.Failure)
            {
                result.Value.StatusAlert = StatusAlertViewExtension.Get(result);
                return View(result.Value);
            }
            result.Value.StatusAlert = StatusAlertViewExtension.Get("Authenticator added.");
           
             return View("TwoFactorAuthenticator", result.Value); 
        }

        [HttpGet]
        public async Task<IActionResult> DisableTwoFactorAuthenticator()
        {
            return await DisableTwoFactorAuthenticatorPost();
        }

        [HttpPost]
        public async Task<IActionResult> DisableTwoFactorAuthenticatorPost()
        {
            TwoFactorAuthenticatorViewModel viewModel;


            Result result = await _twoFactorAuthService.Disable(GetUserId());
            if(result.Failure)
            {
                viewModel = new TwoFactorAuthenticatorViewModel(StatusAlertViewExtension.Get(result));
                ModelState.AddErrors(result.Errors);
                return View(viewModel);
            }

            TwoFactorAuthenticatorViewModel twoFaViewModel = new TwoFactorAuthenticatorViewModel(StatusAlertViewExtension.Get("Two factor authentication disabled."));
            return View("TwoFactorAuthenticator", twoFaViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ResetTwoFactorAuthenticator()
        {
            return await ResetTwoFactorAuthenticatorPost();
        }

        [HttpPost]
        public async Task<IActionResult> ResetTwoFactorAuthenticatorPost()
        {
            TwoFactorAuthenticatorViewModel viewModel;

            Result result = await _twoFactorAuthService.Reset(GetUserId(), GetSessionCode(), GetIp());
            if(result.Failure)
            {
                viewModel = new TwoFactorAuthenticatorViewModel(StatusAlertViewExtension.Get(result));
                ModelState.AddErrors(result.Errors);
                return View(viewModel);
            }

            TwoFactorAuthenticatorViewModel twoFaViewModel = new TwoFactorAuthenticatorViewModel(StatusAlertViewExtension.Get("Two factor authentication reset."));
            return View("TwoFactorAuthenticator", twoFaViewModel);
        }

      
    }
}
