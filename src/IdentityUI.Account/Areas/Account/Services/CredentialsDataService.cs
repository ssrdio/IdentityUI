using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using SSRD.IdentityUI.Account.Areas.Account.Models.Manage;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Services
{
    internal class CredentialsDataService : ICredentialsDataService
    {
        private readonly UserManager<AppUserEntity> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CredentialsDataService(UserManager<AppUserEntity> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CredentailsViewModel> GetViewModel()
        {
            string userId = _httpContextAccessor.HttpContext.User.GetUserId();

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
