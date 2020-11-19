using Microsoft.AspNetCore.Identity;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using SSRD.IdentityUI.Account.Areas.Account.Models.Manage;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Services
{
    internal class CredentialsDataService : ICredentialsDataService
    {
        private readonly UserManager<AppUserEntity> _userManager;
        private readonly IIdentityUIUserInfoService _identityUIUserInfoService;

        public CredentialsDataService(UserManager<AppUserEntity> userManager, IIdentityUIUserInfoService identityUIUserInfoService)
        {
            _userManager = userManager;
            _identityUIUserInfoService = identityUIUserInfoService;
        }

        public async Task<CredentailsViewModel> GetViewModel()
        {
            string userId = _identityUIUserInfoService.GetUserId();

            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);

            IList<UserLoginInfo> logins = await _userManager.GetLoginsAsync(appUser);

            CredentailsViewModel credentailsViewModel = new CredentailsViewModel(
                hasPassword: !string.IsNullOrEmpty(appUser.PasswordHash),
                hasExternalLoginProvider: logins.Count > 0,
                externalLoginProvider: logins.Select(x => x.ProviderDisplayName).SingleOrDefault());

            return credentailsViewModel;
        }
    }
}
