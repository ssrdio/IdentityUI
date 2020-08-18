using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRD.AdminUI.Template.Models;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using SSRD.IdentityUI.Account.Areas.Account.Models;
using SSRD.IdentityUI.Account.Areas.Account.Models.Manage;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.Credentials.Models;

namespace SSRD.IdentityUI.Account.Areas.Account.Controllers
{
    [Route("/[area]/Manage/[controller]/[action]")]
    public class CredentialsController : BaseController
    {
        private const string STATUS_ALERT_TEMP_DATA_KEY = "StatusAlert";

        private readonly ICredentialsService _credentialsService;
        private readonly ICredentialsDataService _credentialsDataService;

        public CredentialsController(ICredentialsService credentialsService, ICredentialsDataService credentialsDataService)
        {
            _credentialsService = credentialsService;
            _credentialsDataService = credentialsDataService;
        }

        [HttpGet("/[area]/Manage/[controller]/")]
        public async Task<IActionResult> Index()
        {
            CredentailsViewModel credentailsViewModel = await _credentialsDataService.GetViewModel();

            StatusAlertViewModel statusAlert = GetTempData<StatusAlertViewModel>(STATUS_ALERT_TEMP_DATA_KEY);
            if (statusAlert != null)
            {
                ModelState.AddErrors(statusAlert.ValidationErrors);
                credentailsViewModel.StatusAlert = statusAlert;
            }

            return View(credentailsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            Result result = await _credentialsService.ChangePassword(GetUserId(), GetSessionCode(), GetIp(), request);
            if (result.Failure)
            {
                SaveTempData(STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get(result));
            }
            else
            {
                SaveTempData(STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get("Password was updated"));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePassword(CreatePasswordRequest createPassword)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            Result result = await _credentialsService.CreatePassword(createPassword);
            if (result.Failure)
            {
                SaveTempData(STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get(result));
            }
            else
            {
                SaveTempData(STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get("Password was created"));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> RemoveExternalProvider()
        {
            Result result = await _credentialsService.RemoveExternalLogin();
            if (result.Failure)
            {
                SaveTempData(STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get(result));
            }
            else
            {
                SaveTempData(STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get("External provider was removed"));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
