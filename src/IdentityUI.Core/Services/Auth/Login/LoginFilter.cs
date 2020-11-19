using Microsoft.Extensions.Logging;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using System;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Auth.Login
{
    public class LoginFilter : ILoginFilter
    {
        private string CAN_NOT_LOGIN = "can_not_login";

        protected ILogger<LoginFilter> _logger;

        public LoginFilter(ILogger<LoginFilter> logger)
        {
            _logger = logger;
        }

        public virtual Task<Result> BeforeAdd(AppUserEntity appUserEntity)
        {
            if (!appUserEntity.Enabled)
            {
                _logger.LogError($"Disabled user can not login. User id {appUserEntity.Id}");
                return Task.FromResult(Result.Fail(CAN_NOT_LOGIN));
            }

            return Task.FromResult(Result.Ok());
        }

        public Task<Result> AfterAdded(AppUserEntity appUserEntity)
        {
            return Task.FromResult(Result.Ok());
        }
    }
}
