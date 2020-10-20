using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Models.Options;

namespace SSRD.IdentityUI.Core.Services.Identity
{
    public class CustomSecurityStampValidator : SecurityStampValidator<AppUserEntity>
    {
        private readonly ISessionService _sessionService;
        private readonly IdentityUIClaimOptions _identityUIClaimOptions;

        private readonly ILogger<CustomSecurityStampValidator> _logger;

#if NET_CORE2
        public CustomSecurityStampValidator(
            IOptions<SecurityStampValidatorOptions> options,
            SignInManager<AppUserEntity> signInManager,
            ISystemClock clock,
            ISessionService sessionService,
            IOptions<IdentityUIClaimOptions> identityUIClaimOptions,
            ILogger<CustomSecurityStampValidator> logger)
            : base(options, signInManager, clock)
        {
            _sessionService = sessionService;
            _identityUIClaimOptions = identityUIClaimOptions.Value;

            _logger = logger;
        }
#endif
#if NET_CORE3
        public CustomSecurityStampValidator(
            IOptions<SecurityStampValidatorOptions> options,
            SignInManager<AppUserEntity> signInManager,
            ISystemClock clock,
            ILoggerFactory logger,
            ISessionService sessionService,
            IOptions<IdentityUIClaimOptions> identityUIClaimOptions)
            : base(options, signInManager, clock, logger)
        {
            _sessionService = sessionService;
            _identityUIClaimOptions = identityUIClaimOptions.Value;

            _logger = logger.CreateLogger<CustomSecurityStampValidator>();
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
                AppUserEntity appUser = await VerifySecurityStamp(context.Principal);
                if (appUser != null)
                {
                    string sessionCode = context.Principal.GetSessionCode(_identityUIClaimOptions);
                    string userId = context.Principal.GetUserId(_identityUIClaimOptions);
                    string impersonatorId = context.Principal.GetImpersonatorId(_identityUIClaimOptions);
                    string ip = context.HttpContext.GetRemoteIp();

                    appUser.SessionCode = sessionCode;
                    appUser.ImpersonatorId = impersonatorId;

                    bool sessionResult = _sessionService.Validate(sessionCode, userId, ip);
                    if(sessionResult)
                    {
                        await SecurityStampVerified(appUser, context);

                        return;
                    }
                }

                _logger.LogInformation($"Invalid security stamp");

                context.RejectPrincipal();
                await SignInManager.SignOutAsync();
            }
        }
    }
}
