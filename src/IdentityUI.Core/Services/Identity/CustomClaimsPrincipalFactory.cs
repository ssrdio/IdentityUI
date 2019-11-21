using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Identity
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUserEntity, RoleEntity>
    {
        public CustomClaimsPrincipalFactory(UserManager<AppUserEntity> userManager, RoleManager<RoleEntity> roleManager,
            IOptions<IdentityOptions> identityOptions) : base(userManager, roleManager, identityOptions)
        {

        }

        public async override Task<ClaimsPrincipal> CreateAsync(AppUserEntity user)
        {
            ClaimsPrincipal claimsPrincipal = await base.CreateAsync(user);

            if (!string.IsNullOrEmpty(user.SessionCode))
            {
                ((ClaimsIdentity)claimsPrincipal.Identity).AddClaim(new Claim(IdentityManagementClaims.SESSION_CODE, user.SessionCode));
            }

            return claimsPrincipal;
        }
    }
}
