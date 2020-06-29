using SSRD.IdentityUI.Core.Data.Entities.Identity;
using System.Collections.Generic;
using System.Linq;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment.Updates
{
    internal class Update_03_UserActiveTwoFactorAuthenticationColumnAdded : AbstractUpdate
    {
        protected override string migrationId => "20200610085725_UserActiveTwoFactorAuthenticationColumnAdded";

        public override int GetVersion()
        {
            return 3;
        }

        public override void AfterSchemaChange(IdentityDbContext context)
        {
            int start = 0;
            int take = 1000;

            while(true)
            {
                List<AppUserEntity> appUsers = context.Users
                    .Where(x => x.TwoFactorEnabled == true)
                    .Skip(start)
                    .Take(take)
                    .ToList();

                foreach (AppUserEntity appUser in appUsers)
                {
                    appUser.TwoFactor = Core.Services.Auth.TwoFactorAuth.Models.TwoFactorAuthenticationType.Authenticator;
                    context.Users.Update(appUser);
                }

                int changes = context.SaveChanges();
                if (changes != appUsers.Count)
                {
                    throw new System.Exception("Failed to update users authenticator");
                }

                if(appUsers.Count != take)
                {
                    break;
                }
            }
        }
    }
}
