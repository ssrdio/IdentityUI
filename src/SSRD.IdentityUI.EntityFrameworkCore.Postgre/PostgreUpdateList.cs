using SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment;
using SSRD.IdentityUI.EntityFrameworkCore.Postgre.Updates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRD.IdentityUI.EntityFrameworkCore.Postgre
{
    internal class PostgreUpdateList : IUpdateList
    {
        private readonly List<IUpdate> _updates = new List<IUpdate>()
        {
            new Update_20191105162011_InitialCreate(),
            new Update_20200512083339_AddPermissionsAddGroups(),
            new Update_20200610085725_UserActiveTwoFactorAuthenticationColumnAdded(),
            new Update_20200811085508_AddUserImageTable(),
            new Update_20200831195154_AddUserAttributes(),
            new Update_20201012130535_AddAudit(),
            new Update_20201029131921_AuditMetadataGroupIdentifier(),
            new Update_20201103064212_AppUserGroupSoftDelete(),
        };

        public IEnumerable<IUpdate> Get()
        {
            return _updates
                .OrderBy(x => x.MigrationId);
        }
    }
}
