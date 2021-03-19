using Microsoft.EntityFrameworkCore;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Infrastructure.Data;
using SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth.Models;
using SSRD.IdentityUI.EntityFrameworkCore.Postgre;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSRD.IdentityUI.EntityFrameworkCore.Postgre.Updates
{
    internal class Update_20200610085725_UserActiveTwoFactorAuthenticationColumnAdded : PostgreUpdate
    {
        public override string MigrationId => "20200610085725_UserActiveTwoFactorAuthenticationColumnAdded";

        public override void AfterSchemaChange(IdentityDbContext context)
        {
            FormattableString updateScript = $@"
                UPDATE ""Users""
                SET ""TwoFactor""={(int)TwoFactorAuthenticationType.Authenticator}
                WHERE ""TwoFactorEnabled""=true";

#if NET_CORE2

            RawSqlString rawSqlString = updateScript;

            context.Database.ExecuteSqlCommand(rawSqlString);
#else
            context.Database.ExecuteSqlInterpolated(updateScript);
#endif
        }
    }
}
