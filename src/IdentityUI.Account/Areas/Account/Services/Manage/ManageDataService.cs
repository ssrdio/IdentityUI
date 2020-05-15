using SSRD.IdentityUI.Account.Areas.Account.Models.Manage;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Result;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Services.Manage
{
    internal class ManageDataService : IManageDataService
    {
        private readonly ITwoFactorAuthService _twoFactorAuthService;
        private readonly IUserRepository _userRepository;

        private readonly ILogger<ManageDataService> _logger;

        public ManageDataService(ITwoFactorAuthService twoFactorAuthService, IUserRepository userRepository, ILogger<ManageDataService> logger)
        {
            _twoFactorAuthService = twoFactorAuthService;
            _userRepository = userRepository;

            _logger = logger;
        }

        public async Task<Result<AddTwoFactorAuthenticatorViewModel>> GetAddTwoFactorAuthenticatorViewModel(string userId, string sessionCode, string ip)
        {
            Result<(string sharedKey, string authenticatorUri)> result = await _twoFactorAuthService.Generate2faCode(userId, sessionCode, ip);
            if(result.Failure)
            {
                return Result.Fail<AddTwoFactorAuthenticatorViewModel>(result.Errors);
            }

            (string sharedKey, string authenticatorUri) = result.Value;

            AddTwoFactorAuthenticatorViewModel model = new AddTwoFactorAuthenticatorViewModel(
                sharedKey: sharedKey,
                authenticationUri: authenticatorUri);

            return Result.Ok(model);
        }

        public Result<ProfileViewModel> GetProfile(string userId)
        {
            SelectSpecification<AppUserEntity, ProfileViewModel> selectSpecification = new SelectSpecification<AppUserEntity, ProfileViewModel>();
            selectSpecification.AddFilter(x => x.Id == userId);
            selectSpecification.AddSelect(x => new ProfileViewModel(
                x.UserName,
                x.FirstName,
                x.LastName,
                x.PhoneNumber));

            ProfileViewModel profile = _userRepository.SingleOrDefault(selectSpecification);
            if(profile == null)
            {
                _logger.LogWarning($"No user. UserId {userId}");
                return Result.Fail<ProfileViewModel>("no_user", "No User");
            }

            return Result.Ok(profile);
        }

        public Result<TwoFactorAuthenticatorViewModel> GetTwoFactorAuthenticatorViewModel(string userId)
        {
            SelectSpecification<AppUserEntity, TwoFactorAuthenticatorViewModel> selectSpecification = new SelectSpecification<AppUserEntity, TwoFactorAuthenticatorViewModel>();
            selectSpecification.AddFilter(x => x.Id == userId);
            selectSpecification.AddSelect(x => new TwoFactorAuthenticatorViewModel(
                x.TwoFactorEnabled));

            TwoFactorAuthenticatorViewModel twoFactorAuthenticatorViewModel = _userRepository.SingleOrDefault(selectSpecification);
            if(twoFactorAuthenticatorViewModel == null)
            {
                _logger.LogWarning($"No user. UserId {userId}");
                return Result.Fail<TwoFactorAuthenticatorViewModel>("no_user", "No User");
            }

            return Result.Ok(twoFactorAuthenticatorViewModel);
        }
    }
}
