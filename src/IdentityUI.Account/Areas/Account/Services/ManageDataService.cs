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
using Microsoft.Extensions.Options;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Interfaces.Services;

namespace SSRD.IdentityUI.Account.Areas.Account.Services
{
    internal class ManageDataService : IManageDataService
    {
        private readonly IUserRepository _userRepository;

        private readonly ILogger<ManageDataService> _logger;

        public ManageDataService(IUserRepository userRepository, ILogger<ManageDataService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public Result<ProfileViewModel> GetProfile(string userId)
        {
            SelectSpecification<AppUserEntity, ProfileViewModel> selectSpecification = new SelectSpecification<AppUserEntity, ProfileViewModel>();
            selectSpecification.AddFilter(x => x.Id == userId);
            selectSpecification.AddSelect(x => new ProfileViewModel(
                x.UserName,
                x.FirstName,
                x.LastName,
                x.PhoneNumber,
                x.PhoneNumberConfirmed));

            ProfileViewModel profile = _userRepository.SingleOrDefault(selectSpecification);

            if (profile == null)
            {
                _logger.LogWarning($"No user. UserId {userId}");
                return Result.Fail<ProfileViewModel>("no_user", "No User");
            }

            return Result.Ok(profile);
        }
    }
}
