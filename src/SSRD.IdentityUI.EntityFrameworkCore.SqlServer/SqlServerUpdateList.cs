using SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment;
using SSRD.IdentityUI.EntityFrameworkCore.SqlServer.Updates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Providers.SqlServer
{
    internal class SqlServerUpdateList : IUpdateList
    {
        private static readonly List<IUpdate> _updates = new List<IUpdate>()
        {
            new Update_20200824141100_InitialCreate(),
            new Update_20200901071938_UserAttributes(),
            new Update_20201012131856_AddAudit(),
            new Update_20201029132919_AuditMetadataGroupIdentifier(),
            new Update_20201103064633_AppUserGroupSoftDelete(),
        };

        public IEnumerable<IUpdate> Get()
        {
            return _updates
                .OrderBy(x => x.MigrationId);
        }
    }
}
