using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using SSRD.IdentityUI.Account.Areas.Account.Models.Manage;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth.Models;
using SSRD.IdentityUI.Core.Services.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Services
{
    class TwoFactorAuthenticationDataService : ITwoFactorAuthenticationDataService
    {
        private readonly UserManager<AppUserEntity> _userManager;
        private readonly IBaseRepository<AppUserEntity> _userRepository;

        private readonly ITwoFactorAuthService _twoFactorAuthService;
        private readonly IIdentityUIUserInfoService _identityUIUserInfoService;

        private readonly IdentityUIEndpoints _options;

        private readonly ILogger<TwoFactorAuthenticationDataService> _logger;

        public TwoFactorAuthenticationDataService(
            UserManager<AppUserEntity> userManager,
            IBaseRepository<AppUserEntity> userRepository,
            ITwoFactorAuthService twoFactorAuthService,
            IIdentityUIUserInfoService identityUIUserInfoService,
            IOptions<IdentityUIEndpoints> options,
            ILogger<TwoFactorAuthenticationDataService> logger)
        {
            _userManager = userManager;
            _userRepository = userRepository;

            _twoFactorAuthService = twoFactorAuthService;
            _identityUIUserInfoService = identityUIUserInfoService;

            _options = options.Value;

            _logger = logger;
        }

        private async Task<Result<AppUserEntity>> GetAppUser()
        {
            string userId = _identityUIUserInfoService.GetUserId();

            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                _logger.LogError($"No user. UserId {userId}");
                return Result.Fail<AppUserEntity>("No User", "no_user");
            }

            if (appUser.TwoFactorEnabled)
            {
                _logger.LogError($"User already has TwoFactorAuthentication enabled. UserId {userId}");
                return Result.Fail<AppUserEntity>("2fa_already_enabled", "Two factor authentication is already enabled");
            }

            return Result.Ok(appUser);
        }

        public async Task<Result<AddPhoneTwoFactorAuthenticationViewModel>> GetPhoneViewModel()
        {
            Result<AppUserEntity> getAppUserResult = await GetAppUser();
            if(getAppUserResult.Failure)
            {
                return Result.Fail<AddPhoneTwoFactorAuthenticationViewModel>(getAppUserResult.Errors);
            }

            AppUserEntity appUser = getAppUserResult.Value;

            if(string.IsNullOrEmpty(appUser.PhoneNumber))
            {
                _logger.LogError($"User does not have a phone number");
                return Result.Fail<AddPhoneTwoFactorAuthenticationViewModel>("no_phone_number", "No Phone number");
            }

            AddPhoneTwoFactorAuthenticationViewModel addPhoneTwoFactorAuthenticationViewModel = new AddPhoneTwoFactorAuthenticationViewModel(
                phoneNumber: appUser.PhoneNumber);

            return Result.Ok(addPhoneTwoFactorAuthenticationViewModel);
        }

        public async Task<Result<AddEmailTwoFactorAuthenticationViewModel>> GetEmailViewModel()
        {
            Result<AppUserEntity> getAppUserResult = await GetAppUser();
            if (getAppUserResult.Failure)
            {
                return Result.Fail<AddEmailTwoFactorAuthenticationViewModel>(getAppUserResult.Errors);
            }

            AppUserEntity appUser = getAppUserResult.Value;

            if (string.IsNullOrEmpty(appUser.Email))
            {
                _logger.LogError($"User does not have a phone number");
                return Result.Fail<AddEmailTwoFactorAuthenticationViewModel>("no_email_address", "No Email address");
            }

            AddEmailTwoFactorAuthenticationViewModel emailTwoFactorAuthenticationViewModel = new AddEmailTwoFactorAuthenticationViewModel();

            return Result.Ok(emailTwoFactorAuthenticationViewModel);
        }

        public async Task<Result<AddTwoFactorAuthenticatorViewModel>> GetAuthenticatorViewModel()
        {
            Result<AppUserEntity> getAppUserResult = await GetAppUser();
            if (getAppUserResult.Failure)
            {
                return Result.Fail<AddTwoFactorAuthenticatorViewModel>(getAppUserResult.Errors);
            }

            AppUserEntity appUser = getAppUserResult.Value;

            Result<(string sharedKey, string authenticatorUri)> result = await _twoFactorAuthService.Generate2faCode(appUser.Id);
            if (result.Failure)
            {
                return Result.Fail<AddTwoFactorAuthenticatorViewModel>(result.Errors);
            }

            (string sharedKey, string authenticatorUri) = result.Value;

            AddTwoFactorAuthenticatorViewModel model = new AddTwoFactorAuthenticatorViewModel(
                sharedKey: sharedKey,
                authenticationUri: authenticatorUri);

            return Result.Ok(model);
        }

        public Result<TwoFactorAuthenticatorViewModel> GetTwoFactorAuthenticatorViewModel()
        {
            string userId = _identityUIUserInfoService.GetUserId();

            SelectSpecification<AppUserEntity, TwoFactorAuthenticatorViewModel> selectSpecification = new SelectSpecification<AppUserEntity, TwoFactorAuthenticatorViewModel>();
            selectSpecification.AddFilter(x => x.Id == userId);
            selectSpecification.AddSelect(x => new TwoFactorAuthenticatorViewModel(
                x.TwoFactorEnabled,
                x.TwoFactor.ToProvider(),
                _options.UseSmsGateway && !string.IsNullOrEmpty(x.PhoneNumber),
                _options.UseEmailSender.GetValueOrDefault(false) && !string.IsNullOrEmpty(x.Email)));

            TwoFactorAuthenticatorViewModel twoFactorAuthenticatorViewModel = _userRepository.SingleOrDefault(selectSpecification);
            if (twoFactorAuthenticatorViewModel == null)
            {
                _logger.LogWarning($"No user. UserId {userId}");
                return Result.Fail<TwoFactorAuthenticatorViewModel>("no_user", "No User");
            }

            return Result.Ok(twoFactorAuthenticatorViewModel);
        }
    }
}
