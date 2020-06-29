using Microsoft.AspNetCore.Mvc;
using SSRD.AdminUI.Template.Models;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using SSRD.IdentityUI.Account.Areas.Account.Models;
using SSRD.IdentityUI.Account.Areas.Account.Models.Manage;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Controllers
{
    public class TwoFactorAuthenticationController : BaseController
    {
        private const string INDEX_STATUS_ALERT_TEMP_DATA_KEY = "IndexStatusAlert";

        private const string EMAIL_STATUS_ALERT_TEMP_DATA_KEY = "EmailStatusAlert";
        private const string PHONE_STATUS_ALERT_TEMP_DATA_KEY = "PhoneStatusAlert";
        private const string AUTHENTICATOR_STATUS_ALERT_TEMP_DATA_KEY = "AuthenticatorStatusAlert";

        private const string RECOVERY_CODES_KEY = "RecoveryCodes";

        private readonly ITwoFactorAuthenticationDataService _twoFactorAuthorizationDataService;
        private readonly ITwoFactorAuthService _twoFactorAuthService;

        public TwoFactorAuthenticationController(ITwoFactorAuthenticationDataService twoFactorAuthorizationDataService, ITwoFactorAuthService twoFactorAuthService)
        {
            _twoFactorAuthorizationDataService = twoFactorAuthorizationDataService;
            _twoFactorAuthService = twoFactorAuthService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            Result<TwoFactorAuthenticatorViewModel> result = _twoFactorAuthorizationDataService.GetTwoFactorAuthenticatorViewModel();
            if (result.Failure)
            {
                return View(new TwoFactorAuthenticatorViewModel(StatusAlertViewExtension.Get(result)));
            }

            TwoFactorAuthenticatorViewModel twoFactorAuthenticatorViewModel = result.Value;

            StatusAlertViewModel statusAlert = GetTempData<StatusAlertViewModel>(INDEX_STATUS_ALERT_TEMP_DATA_KEY);
            if (statusAlert != null)
            {
                ModelState.AddErrors(statusAlert.ValidationErrors);
                twoFactorAuthenticatorViewModel.StatusAlert = statusAlert;
            }

            return View(twoFactorAuthenticatorViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AddTwoFactorEmailAuthentication()
        {
            Result<AddEmailTwoFactorAuthenticationViewModel> result = await _twoFactorAuthorizationDataService.GetEmailViewModel();
            if(result.Failure)
            {
                SaveTempData(INDEX_STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get(result));
                return RedirectToAction(nameof(Index));
            }

            AddEmailTwoFactorAuthenticationViewModel addEmailTwoFactorAuthenticationViewModel = new AddEmailTwoFactorAuthenticationViewModel();

            StatusAlertViewModel statusAlert = GetTempData<StatusAlertViewModel>(EMAIL_STATUS_ALERT_TEMP_DATA_KEY);
            if (statusAlert != null)
            {
                ModelState.AddErrors(statusAlert.ValidationErrors);
                addEmailTwoFactorAuthenticationViewModel.StatusAlert = statusAlert;
            }

            Result sendMailResult = await _twoFactorAuthService.GenerateAndSendEmailCode(GetUserId());

            return View(addEmailTwoFactorAuthenticationViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SendTwoFactorEmailAuthentication()
        {
            Result result = await _twoFactorAuthService.GenerateAndSendEmailCode(GetUserId());
            if (result.Failure)
            {
                SaveTempData(EMAIL_STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get(result));
            }
            else
            {
                SaveTempData(EMAIL_STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get("Code was send."));
            }

            return RedirectToAction(nameof(AddTwoFactorEmailAuthentication));
        }

        [HttpPost]
        public async Task<IActionResult> AddTwoFactorEmailAuthentication(AddTwoFactorEmailAuthenticationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            Result<IEnumerable<string>> verifyResult = await _twoFactorAuthService.VerifyEmailTwoFactorCode(GetUserId(), request);
            if (verifyResult.Failure)
            {
                SaveTempData(EMAIL_STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get(verifyResult));

                return RedirectToAction(nameof(AddTwoFactorEmailAuthentication));
            }

            SaveTempData(RECOVERY_CODES_KEY, verifyResult.Value);
            return RedirectToAction(nameof(RecoveryCodesView));
        }

        [HttpGet]
        public async Task<IActionResult> AddTwoFactorPhoneAuthentication()
        {
            Result<AddPhoneTwoFactorAuthenticationViewModel> result = await _twoFactorAuthorizationDataService.GetPhoneViewModel();
            if (result.Failure)
            {
                SaveTempData(INDEX_STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get(result));
                return RedirectToAction(nameof(Index));
            }

            AddPhoneTwoFactorAuthenticationViewModel addPhoneTwoFactorAuthenticationViewModel = result.Value;

            StatusAlertViewModel statusAlert = GetTempData<StatusAlertViewModel>(PHONE_STATUS_ALERT_TEMP_DATA_KEY);
            if (statusAlert != null)
            {
                ModelState.AddErrors(statusAlert.ValidationErrors);
                addPhoneTwoFactorAuthenticationViewModel.StatusAlert = statusAlert;
            }

            Result sendSmsResult = await _twoFactorAuthService.GenerateSmsCode(GetUserId());

            return View(addPhoneTwoFactorAuthenticationViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddTwoFactorPhoneAuthentication(AddTwoFactorPhoneAuthenticationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            Result<IEnumerable<string>> verifyResult = await _twoFactorAuthService.VerifyPhoneTwoFactorCode(GetUserId(), request);
            if (verifyResult.Failure)
            {
                SaveTempData(PHONE_STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get(verifyResult));

                return RedirectToAction(nameof(AddTwoFactorEmailAuthentication));
            }

            SaveTempData(RECOVERY_CODES_KEY, verifyResult.Value);
            return RedirectToAction(nameof(RecoveryCodesView));
        }

        [HttpPost]
        public async Task<IActionResult> SendTwoFactorPhoneAuthentication()
        {
            Result result = await _twoFactorAuthService.GenerateSmsCode(GetUserId());
            if (result.Failure)
            {
                SaveTempData(PHONE_STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get(result));
            }
            else
            {
                SaveTempData(PHONE_STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get("Code was send."));
            }

            return RedirectToAction(nameof(AddTwoFactorPhoneAuthentication));
        }

        [HttpGet]
        public async Task<IActionResult> AddTwoFactorAuthenticator()
        {
            Result<AddTwoFactorAuthenticatorViewModel> result = await _twoFactorAuthorizationDataService.GetAuthenticatorViewModel();
            if (result.Failure)
            {
                SaveTempData(INDEX_STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get(result));
                return RedirectToAction(nameof(Index));
            }

            AddTwoFactorAuthenticatorViewModel addTwoFactorAuthenticator = result.Value;

            StatusAlertViewModel statusAlert = GetTempData<StatusAlertViewModel>(AUTHENTICATOR_STATUS_ALERT_TEMP_DATA_KEY);
            if (statusAlert != null)
            {
                ModelState.AddErrors(statusAlert.ValidationErrors);
                addTwoFactorAuthenticator.StatusAlert = statusAlert;
            }

            return View(addTwoFactorAuthenticator);
        }

        [HttpPost]
        public async Task<IActionResult> AddTwoFactorAuthenticator(AddTwoFactorAuthenticatorRequest request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            string userId = GetUserId();

            Result<IEnumerable<string>> verifyResult = await _twoFactorAuthService.VerifyTwoFactorCode(userId, request);
            if (verifyResult.Failure)
            {
                SaveTempData(AUTHENTICATOR_STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get(verifyResult));

                return RedirectToAction(nameof(AddTwoFactorAuthenticator));
            }

            SaveTempData(RECOVERY_CODES_KEY, verifyResult.Value);
            return RedirectToAction(nameof(RecoveryCodesView));
        }

        [HttpGet]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            return await DisableTwoFactorAuthenticationPost();
        }

        [HttpPost]
        public async Task<IActionResult> DisableTwoFactorAuthenticationPost()
        {
            Result result = await _twoFactorAuthService.Disable(GetUserId());
            if (result.Failure)
            {
                SaveTempData(INDEX_STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get(result));
            }
            else
            {
                SaveTempData(INDEX_STATUS_ALERT_TEMP_DATA_KEY, StatusAlertViewExtension.Get("Two factor authentication disabled."));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult RecoveryCodesView()
        {
            List<string> recoveryCodes = GetTempData<List<string>>(RECOVERY_CODES_KEY);
            if(recoveryCodes == null)
            {
                recoveryCodes = new List<string>();
            }

            TwoFactorRecoverCodesViewModel model = new TwoFactorRecoverCodesViewModel(recoveryCodes);
            
            return View(model);
        }
    }
}