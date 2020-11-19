using FluentValidation;
using FluentValidation.Results;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.Login.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth.Models;
using Microsoft.AspNetCore.Http;
using SSRD.IdentityUI.Core.Services.Identity;

namespace SSRD.IdentityUI.Core.Services.Auth
{
    internal class LoginService : ILoginService
    {
        private readonly SignInManager<AppUserEntity> _signInManager;
        private readonly UserManager<AppUserEntity> _userManager;

        private readonly ISessionService _sessionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoginFilter _canLoginService;

        private readonly IValidator<LoginRequest> _loginValidator;
        private readonly IValidator<LoginWith2faRequest> _lginwith2faValidator;
        private readonly IValidator<LoginWithRecoveryCodeRequest> _loginWithRecoveryCodeValidator;

        private readonly ILogger<LoginService> _logger;

        public LoginService(
            SignInManager<AppUserEntity> signInManager,
            UserManager<AppUserEntity> userManager,
            ISessionService sessionService,
            IHttpContextAccessor httpContextAccessor,
            ILoginFilter canLoginService,
            IValidator<LoginRequest> loginValidator,
            IValidator<LoginWith2faRequest> loginWith2faValidator,
            IValidator<LoginWithRecoveryCodeRequest> loginWithRecoveryCodeValidator,
            ILogger<LoginService> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;

            _httpContextAccessor = httpContextAccessor;
            _sessionService = sessionService;
            _canLoginService = canLoginService;

            _loginValidator = loginValidator;
            _lginwith2faValidator = loginWith2faValidator;
            _loginWithRecoveryCodeValidator = loginWithRecoveryCodeValidator;

            _logger = logger;
        }

        public Task<SignInResult> Login(LoginRequest login)
        {
            string ip = _httpContextAccessor.HttpContext.GetRemoteIp();
            string sessionCode = _httpContextAccessor.HttpContext.User.GetSessionCode();

            return Login(ip, sessionCode, login);
        }

        public async Task<SignInResult> Login(string ip, string sessionCode, LoginRequest login)
        {
            ValidationResult validationResult = _loginValidator.Validate(login);
            if(!validationResult.IsValid)
            {
                _logger.LogError($"Invalid LoginRequest. UserName {login?.UserName}");
                return SignInResult.Failed;
            }

            await _signInManager.SignOutAsync();

            AppUserEntity appUser = await _userManager.FindByNameAsync(login.UserName);
            if(appUser == null)
            {
                _logger.LogInformation($"No user with username {login.UserName}");
                return SignInResult.Failed;
            }

            if (sessionCode != null)
            {
                _sessionService.Logout(sessionCode, appUser.Id, SessionEndTypes.Expired);
            }

            CommonUtils.Result.Result beforeLoginfilterResult = await _canLoginService.BeforeAdd(appUser);
            if (beforeLoginfilterResult.Failure)
            {
                _logger.LogInformation($"User is not allowed to login. User {appUser.Id}");
                return SignInResult.Failed;
            }

            appUser.SessionCode = Guid.NewGuid().ToString();

            Result addSessionResult = _sessionService.Add(appUser.SessionCode, appUser.Id, ip);
            if(addSessionResult.Failure)
            {
                return SignInResult.Failed;
            }

            SignInResult result = await _signInManager.PasswordSignInAsync(appUser, login.Password, login.RememberMe, lockoutOnFailure: true);
            if(!result.Succeeded)
            {
                if(result.RequiresTwoFactor)
                {
                    _logger.LogInformation($"Login Requires TwoFactor. User {appUser.Id}");
                    _sessionService.Logout(appUser.SessionCode, appUser.Id, SessionEndTypes.TwoFactorLogin);
                }

                if (!result.IsLockedOut)
                {
                    _logger.LogInformation($"Failed to log in user. User {appUser.Id}");
                    _sessionService.Logout(appUser.SessionCode, appUser.Id, SessionEndTypes.InvalidLogin);
                }

                return result;
            }

            CommonUtils.Result.Result afterLoginFilterResult = await _canLoginService.AfterAdded(appUser);
            if (afterLoginFilterResult.Failure)
            {
                await _signInManager.SignOutAsync();
                _sessionService.Logout(appUser.SessionCode, appUser.Id, SessionEndTypes.AffterLoginFilterFailure);

                return SignInResult.Failed;
            }

            _logger.LogInformation($"User id logged in. UserId {appUser.Id}");

            return result;
        }

        /// <summary>
        /// Used to login user after password change, 2fa change
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<Result> Login(string userId)
        {
            string ip = _httpContextAccessor.HttpContext.GetRemoteIp();
            string sessionCode = _httpContextAccessor.HttpContext.User.GetSessionCode();

            return Login(userId, sessionCode, ip);
        }

        /// <summary>
        /// Used to login user after password change, 2fa change
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionCode"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public async Task<Result> Login(string userId, string sessionCode, string ip)
        {
            await _signInManager.SignOutAsync();

            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                _logger.LogInformation($"No user with username {userId}");
                return Result.Fail("error", "Error");
            }

            if (sessionCode != null)
            {
                _sessionService.Logout(sessionCode, appUser.Id, SessionEndTypes.SecurityCodeChange);
            }

            CommonUtils.Result.Result canLoginResult = await _canLoginService.BeforeAdd(appUser);
            if (canLoginResult.Failure)
            { 
                _logger.LogInformation($"User is not allowd to login. User {appUser.Id}");
                return Result.Fail("error", "Error");
            }

            appUser.SessionCode = Guid.NewGuid().ToString();

            Result addSessionResult = _sessionService.Add(appUser.SessionCode, appUser.Id, ip);
            if (addSessionResult.Failure)
            {
                return Result.Fail("error", "error");
            }

            await _signInManager.SignInAsync(appUser, false);

            CommonUtils.Result.Result afterLoginFilterResult = await _canLoginService.AfterAdded(appUser);
            if (afterLoginFilterResult.Failure)
            {
                await _signInManager.SignOutAsync();
                _sessionService.Logout(appUser.SessionCode, appUser.Id, SessionEndTypes.AffterLoginFilterFailure);

                return Result.Fail("error", "error");
            }

            _logger.LogInformation($"User loged in. UserId {appUser.Id}");

            return Result.Ok();
        }

        public async Task<SignInResult> LoginWith2fa(string ip, LoginWith2faRequest loginWith2FaRequest)
        {
            ValidationResult validationResult = _lginwith2faValidator.Validate(loginWith2FaRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogError($"Invalid LoginWith2faRequest");
                return SignInResult.Failed;
            }

            string code = loginWith2FaRequest.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            AppUserEntity appUser = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if(appUser == null)
            {
                _logger.LogError($"No user for Twofactor login");
                return SignInResult.Failed;
            }

            CommonUtils.Result.Result canLoginResult = await _canLoginService.BeforeAdd(appUser);
            if (canLoginResult.Failure)
            {
                _logger.LogInformation($"User is not allowd to login. User {appUser.Id}");
                return SignInResult.Failed;
            }

            if (!appUser.TwoFactorEnabled || appUser.TwoFactor == TwoFactorAuthenticationType.None)
            {
                _logger.LogError($"Use does not have 2fa enabled. User {appUser.Id}");
                return SignInResult.Failed;
            }

            appUser.SessionCode = Guid.NewGuid().ToString();

            Result addSessionResult = _sessionService.Add(appUser.SessionCode, appUser.Id, ip);
            if (addSessionResult.Failure)
            {
                return SignInResult.Failed;
            }

            SignInResult signInResult = await _signInManager.TwoFactorSignInAsync(appUser.TwoFactor.ToProvider(), code,
                loginWith2FaRequest.RememberMe, loginWith2FaRequest.RememberMachine);

            if(!signInResult.Succeeded)
            {
                _logger.LogError($"Faild to log in user with TwoFactorAuthenticator");
                _sessionService.Logout(appUser.SessionCode, appUser.Id, SessionEndTypes.InvlidTwoFactorLogin);
            }

            CommonUtils.Result.Result afterLoginFilterResult = await _canLoginService.AfterAdded(appUser);
            if (afterLoginFilterResult.Failure)
            {
                await _signInManager.SignOutAsync();
                _sessionService.Logout(appUser.SessionCode, appUser.Id, SessionEndTypes.AffterLoginFilterFailure);

                return SignInResult.Failed;
            }

            _logger.LogInformation($"User logged in with 2fa. UserId {appUser.Id}");

            return signInResult;
        }

        public async Task<Result> LoginWithRecoveryCode(LoginWithRecoveryCodeRequest loginWithRecoveryCode)
        {
            ValidationResult validationResult = _loginWithRecoveryCodeValidator.Validate(loginWithRecoveryCode);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(LoginWithRecoveryCodeRequest)} model");
                return Result.Fail(validationResult.Errors);
            }

            //TODO: check if user can login

            SignInResult loginResult = await _signInManager.TwoFactorRecoveryCodeSignInAsync(loginWithRecoveryCode.RecoveryCode);
            if(!loginResult.Succeeded)
            {
                _logger.LogError($"Failed to login with recovery code");
                return Result.Fail("invalid_recovery_code", "Invalid recovery code");
            }

            return Result.Ok();
        }

        public async Task<Result> Logout(string userId, string sessionCode)
        {
            await _signInManager.SignOutAsync();

            _sessionService.Logout(sessionCode, userId, SessionEndTypes.Logout);

            _logger.LogInformation($"User loged out. UserId {userId}");

            return Result.Ok();
        }
    }
}
