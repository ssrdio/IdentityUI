using System;
using System.Collections.Generic;
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

        private readonly IdentityUIEndpoints _identityUIEndpoints;

        public AccountController(ILoginService loginService, IEmailConfirmationService emailConfirmationService,
            IAddUserService addUserService, ICredentialsService credentialsService, IAccountDataService accountDataService,
            ITwoFactorAuthService twoFactorAuthService, IOptionsSnapshot<IdentityUIEndpoints> identityUIEndpoints)
        {
            _loginService = loginService;
            _emailConfirmationService = emailConfirmationService;
            _addUserService = addUserService;
            _credentialsService = credentialsService;
            _accountDataService = accountDataService;
            _twoFactorAuthService = twoFactorAuthService;

            _identityUIEndpoints = identityUIEndpoints.Value;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            LoginViewModel loginViewModel = _accountDataService.GetLoginViewModel(returnUrl);

            return View(loginViewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                LoginViewModel loginViewModel = _accountDataService.GetLoginViewModel(returnUrl);
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
                return RedirectToAction("LoginWith2fa", new { RememberMe = loginRequest.RememberMe, ReturnUrl = returnUrl});
            }
            else if(result.IsLockedOut)
            {
                return LocalRedirect(PagePath.LOCKOUT);
            }
            else
            {
                LoginViewModel loginViewModel = _accountDataService.GetLoginViewModel(returnUrl);

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

            return RedirectToAction(nameof(RegisterSuccess));
        }
    }
}
