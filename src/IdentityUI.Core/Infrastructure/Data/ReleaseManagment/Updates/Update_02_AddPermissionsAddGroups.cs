using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment.Updates
{
    internal class Update_02_AddPermissionsAddGroups : AbstractUpdate
    {
        protected override string migrationId => "20200512083339_AddPermissionsAddGroups";

        public override void AfterSchemaChange()
        {
        }

        public override void BeforeSchemaChange()
        {
        }

        public override int GetVersion()
        {
            return 2;
        }

        public override void SchemaChange(DatabaseFacade database)
        {
            ExecuteMigrationSql(database, migrationId);
        }
    }
}
