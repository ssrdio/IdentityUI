using SSRD.IdentityUI.Account.Areas.Account.Models.Manage;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Services.Manage
{
    public interface IManageDataService
    {
        Result<ProfileViewModel> GetProfile(string userId);

        Task<Result<AddTwoFactorAuthenticatorViewModel>> GetAddTwoFactorAuthenticatorViewModel(string userId, string sessionCode, string ip);
        Result<TwoFactorAuthenticatorViewModel> GetTwoFactorAuthenticatorViewModel(string userId);
    }
}
