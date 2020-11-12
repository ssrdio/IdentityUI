using System.Linq;
using System.Threading.Tasks;
using SSRD.IdentityUI.Account.Areas.Account.Models;
using SSRD.IdentityUI.Account.Areas.Account.Models.Account;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.Credentials.Models;
using SSRD.IdentityUI.Core.Services.Auth.Login.Models;
using SSRD.IdentityUI.Core.Services.User.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using Microsoft.AspNetCore.Authentication;
using SSRD.IdentityUI.Core.Services.Group.Models;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;
using System.Net.Http;
using System;
using System.Collections.Generic;
using SSRD.AdminUI.Template.Atttributes;

namespace SSRD.IdentityUI.Account.Areas.Account.Controllers
{
    [Route("/[area]/[action]")]
    public class AccountController : BaseController
    {
        private readonly ILoginService _loginService;
        private readonly ITwoFactorAuthService _twoFactorAuthService;
        private readonly IAddUserService _addUserService;
        private readonly IEmailConfirmationService _emailConfirmationService;
        private readonly ICredentialsService _credentialsService;
        private readonly IAccountDataService _accountDataService;
        private readonly IExternalLoginService _externalLoginService;
        private readonly IGroupRegistrationService _groupRegistrationService;

        private readonly IdentityUIEndpoints _identityUIEndpoints;

        public AccountController(
            ILoginService loginService,
            IEmailConfirmationService emailConfirmationService,
            IAddUserService addUserService,
            ICredentialsService credentialsService,
            IAccountDataService accountDataService,
            ITwoFactorAuthService twoFactorAuthService,
            IExternalLoginService externalLoginService,
            IGroupRegistrationService groupRegistrationService,
            IOptionsSnapshot<IdentityUIEndpoints> identityUIEndpoints)
        {
            _loginService = loginService;
            _emailConfirmationService = emailConfirmationService;
            _addUserService = addUserService;
            _credentialsService = credentialsService;
            _accountDataService = accountDataService;
            _twoFactorAuthService = twoFactorAuthService;
            _externalLoginService = externalLoginService;
            _groupRegistrationService = groupRegistrationService;

            _identityUIEndpoints = identityUIEndpoints.Value;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            LoginViewModel loginViewModel = await _accountDataService.GetLoginViewModel(returnUrl);

            return View(loginViewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateReCaptcha(requiredScore: 0.7d, action: "login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                LoginViewModel loginViewModel = await _accountDataService.GetLoginViewModel(returnUrl);
                return View(loginViewModel);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            Microsoft.AspNetCore.Identity.SignInResult result = await _loginService.Login(GetIp(), GetSessionCode(), loginRequest);
            if(result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else if(result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(LoginWith2fa), new { RememberMe = loginRequest.RememberMe, ReturnUrl = returnUrl});
            }
            else if(result.IsLockedOut)
            {
                return LocalRedirect(PagePath.LOCKOUT);
            }
            else
            {
                LoginViewModel loginViewModel = await _accountDataService.GetLoginViewModel(returnUrl);

                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                return View(loginViewModel);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            ViewBag.RetunrUrl = returnUrl;

            LoginWith2faViewModel loginWith2FaViewModel = new LoginWith2faViewModel(
                rememberMe: rememberMe);

            await _twoFactorAuthService.TrySend2faCode(GetUserId());

            return View(loginWith2FaViewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateReCaptcha(requiredScore: 0.7d, action: "login2fa")]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faRequest loginWith2FaRequest, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Login));
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            Microsoft.AspNetCore.Identity.SignInResult result = await _loginService.LoginWith2fa(GetIp(), loginWith2FaRequest);
            if(result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else if(result.IsLockedOut)
            {
                return LocalRedirect(PagePath.LOCKOUT);
            }
            else
            {
                ViewBag.RetunrUrl = returnUrl;
                ModelState.AddModelError(string.Empty, "Invalid authenticator code");

                return View();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult LoginWithRecoveryCode(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            return View(new LoginWithRecoveryCodeViewModel(returnUrl));
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateReCaptcha(requiredScore: 0.7d, action: "loginRecoveryCode")]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeRequest loginWithRecoveryCode, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(new LoginWithRecoveryCodeViewModel(returnUrl));
            }

            Result result = await _loginService.LoginWithRecoveryCode(loginWithRecoveryCode);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return View(new LoginWithRecoveryCodeViewModel(returnUrl));
            }

            return LocalRedirect(returnUrl);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Lockout()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ExternalLoginError()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            Result result = await _loginService.Logout(GetUserId(), GetSessionCode());

            return Redirect("/");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            if (!_identityUIEndpoints.RegisterEnabled)
            {
                return NotFound();
            }

            RegisterViewModel registerViewModel = _accountDataService.GetRegisterViewModel();

            return View(registerViewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateReCaptcha(requiredScore: 0.7d, action: "register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            if(!_identityUIEndpoints.RegisterEnabled)
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                RegisterViewModel registerViewModel = _accountDataService.GetRegisterViewModel();

                return View(registerViewModel);
            }

            Result result = await _addUserService.Register(registerRequest);
            if(result.Failure)
            {
                RegisterViewModel registerViewModel = _accountDataService.GetRegisterViewModel();

                ModelState.AddErrors(result.Errors);
                return View(registerViewModel);
            }

            return RedirectToAction(nameof(RegisterSuccess));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegisterSuccess()
        {
            RegisterSuccessViewModel registerSuccessViewModel = _accountDataService.GetRegisterSuccessViewModel();

            return View(registerSuccessViewModel);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            if(!_identityUIEndpoints.UseEmailSender.HasValue || !_identityUIEndpoints.UseEmailSender.Value)
            {
                return NotFound();
            }

            ResetPasswordViewModel viewModel = new ResetPasswordViewModel(
                code: code);

            return View(viewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateReCaptcha(requiredScore: 0.7d, action: "resetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            if (!_identityUIEndpoints.UseEmailSender.HasValue || !_identityUIEndpoints.UseEmailSender.Value)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Login));
            }

            Result result = await _credentialsService.ResetPassword(request);
            if(result.Failure)
            {
                ViewBag.Code = request.Code;

                ModelState.AddErrors(result.Errors);
                return View();
            }

            return Redirect(PagePath.RESET_PASSWORD_SUCCESS);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPasswordSuccess()
        {
            if (!_identityUIEndpoints.UseEmailSender.HasValue || !_identityUIEndpoints.UseEmailSender.Value)
            {
                return NotFound();
            }

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RecoverPassword()
        {
            if (!_identityUIEndpoints.UseEmailSender.HasValue || !_identityUIEndpoints.UseEmailSender.Value)
            {
                return NotFound();
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateReCaptcha(requiredScore: 0.7d, action: "recoverPassword")]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordRequest request)
        {
            if (!_identityUIEndpoints.UseEmailSender.HasValue || !_identityUIEndpoints.UseEmailSender.Value)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Login));
            }

            Result result = await _credentialsService.RecoverPassword(request);
            if(result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return View();
            }

            return Redirect(PagePath.RECOVER_PASSWORD_SUCCESS);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RecoverPasswordSuccess()
        {
            if (!_identityUIEndpoints.UseEmailSender.HasValue || !_identityUIEndpoints.UseEmailSender.Value)
            {
                return NotFound();
            }

            return View();
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> ConfirmEmail(string id, string code)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            Result result = await _emailConfirmationService.ConfirmEmail(id, code);
            if(result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return View();
            }

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AcceptInvite(string code = null)
        {
            AcceptInviteViewModel viewModel = new AcceptInviteViewModel(
                code: code);

            return View(viewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateReCaptcha(requiredScore: 0.7d, action: "acceptInvite")]
        public async Task<IActionResult> AcceptInvite(AcceptInviteRequest acceptInvite)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Result result = await _addUserService.AcceptInvite(acceptInvite);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return View();
            }

            return RedirectToAction(nameof(AcceptInviteSuccess));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AcceptInviteSuccess()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ExternalLogin(ExternalLoginRequest externalLoginRequest, string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
            {
                LoginViewModel loginViewModel = await _accountDataService.GetLoginViewModel(returnUrl);
                return View(nameof(Login), loginViewModel);
            }

            Result<AuthenticationProperties> result = await _externalLoginService.ExternalLogin(externalLoginRequest, returnUrl);
            if(result.Failure)
            {
                LoginViewModel loginViewModel = await _accountDataService.GetLoginViewModel(returnUrl);
                return View(nameof(Login), loginViewModel);
            }

            return Challenge(result.Value, externalLoginRequest.Provider);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback([FromQuery] string returnUrl, [FromQuery] string remoteError)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            Result<Microsoft.AspNetCore.Identity.SignInResult> proccessCallbackResult = await _externalLoginService.Callback(remoteError);
            if (proccessCallbackResult.Failure)
            {
                return RedirectToAction(nameof(Login), new { ReturnUrl = returnUrl });
            }

            if(proccessCallbackResult.Value.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            if(proccessCallbackResult.Value.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            if(proccessCallbackResult.Value.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(LoginWith2fa), new { ReturnUrl = returnUrl });
            }

            Result<ExternalLoginRegisterViewModel> getViewModelResult = await _accountDataService.GetExternalLoginViewModel(returnUrl);
            if(getViewModelResult.Failure)
            {
                LoginViewModel loginViewModel = await _accountDataService.GetLoginViewModel(returnUrl);
                return View(nameof(Login), loginViewModel);
            }

            return View("ExternalLoginRegister", getViewModelResult.Value);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ExternalLoginRegister(string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            Result<ExternalLoginRegisterViewModel> getViewModelResult = await _accountDataService.GetExternalLoginViewModel(returnUrl);
            if (getViewModelResult.Failure)
            {
                LoginViewModel loginViewModel = await _accountDataService.GetLoginViewModel(returnUrl);
                return View(nameof(Login), loginViewModel);
            }

            return View(getViewModelResult.Value);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateReCaptcha(requiredScore: 0.7d, action: "externalLoginRegister")]
        public async Task<IActionResult> ExternalLoginRegister(ExternalLoginRegisterRequest externalLoginRegisterRequest, string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            Result<ExternalLoginRegisterViewModel> getViewModelResult = await _accountDataService.GetExternalLoginViewModel(returnUrl);
            if (getViewModelResult.Failure)
            {
                //HACK: change to support list of errors
                LoginViewModel loginViewModel = await _accountDataService.GetLoginViewModel(returnUrl, getViewModelResult.Errors.FirstOrDefault()?.Message);
                return View(nameof(Login), loginViewModel);
            }

            if (!ModelState.IsValid)
            {
                return View(getViewModelResult.Value);
            }

            Result result = await _addUserService.ExternalLoginRequest(externalLoginRegisterRequest);
            if(result.Failure)
            {
                getViewModelResult.Value.StatusAlert = StatusAlertViewExtension.Get(result);
                ModelState.AddErrors(result.Errors);
                return View(getViewModelResult.Value);
            }

            return RedirectToAction(nameof(ExternalLoginRegisterSuccess));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ExternalLoginRegisterSuccess()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegisterGroup()
        {
            if (!_identityUIEndpoints.GroupRegistrationEnabled)
            {
                return NotFound();
            }

            RegisterGroupViewModel registerViewModel = _accountDataService.GetRegisterGroupViewModel();

            return View(registerViewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateReCaptcha(requiredScore: 0.7d, action: "registerGroup")]
        public async Task<IActionResult> RegisterGroup(RegisterGroupModel registerGroupModel)
        {
            if (!_identityUIEndpoints.GroupRegistrationEnabled)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                RegisterGroupViewModel registerViewModel = _accountDataService.GetRegisterGroupViewModel();

                return View(registerViewModel);
            }

            CommonUtils.Result.Result result = await _groupRegistrationService.Add(registerGroupModel);
            if (result.Failure)
            {
                RegisterGroupViewModel registerViewModel = _accountDataService.GetRegisterGroupViewModel();

                CommonUtils.Result.ResultExtensions.AddResultErrors(ModelState, result);
                return View(registerViewModel);
            }

            return RedirectToAction(nameof(RegisterSuccess));
        }
    }
}
