using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SSRD.IdentityUI.Core.Services.Identity
{
    public class CustomSecurityStampValidator : SecurityStampValidator<AppUserEntity>
    {
        private readonly ISessionService _sessionService;

#if NET_CORE2
        public CustomSecurityStampValidator(IOptions<SecurityStampValidatorOptions> options, SignInManager<AppUserEntity> signInManager,
            ISystemClock clock, ISessionService sessionService)
            : base(options, signInManager, clock)
        {
            _sessionService = sessionService;
        }
#endif
#if NET_CORE3
        public CustomSecurityStampValidator(IOptions<SecurityStampValidatorOptions> options, SignInManager<AppUserEntity> signInManager,
            ISystemClock clock, ILoggerFactory logger, ISessionService sessionService)
            : base(options, signInManager, clock, logger)
        {
            _sessionService = sessionService;
        }
#endif

        public override async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            if (context.Options != null && Clock != null)
            {
                utcNow = Clock.UtcNow;
            }
            DateTimeOffset? issuedUtc = context.Properties.IssuedUtc;
            bool flag = !issuedUtc.HasValue;
            if (issuedUtc.HasValue)
            {
                flag = (utcNow.Subtract(issuedUtc.Value) > Options.ValidationInterval);
            }
            if (flag)
            {
                AppUserEntity val = await VerifySecurityStamp(context.Principal);
                if (val != null)
                {
                    string sessionCode = context.Principal.FindFirstValue(IdentityManagementClaims.SESSION_CODE);
                    string userId = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                    string ip = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();

                    bool sessionResult = _sessionService.Validate(sessionCode, userId, ip);
                    if(sessionResult)
                    {
                        await SecurityStampVerified(val, context);

                        return;
                    }
                }
                context.RejectPrincipal();
                await SignInManager.SignOutAsync();
            }
        }
    }
}
