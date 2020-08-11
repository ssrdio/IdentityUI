using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment.Updates
{
    internal class Update_04_AddUserImageTable : AbstractUpdate
    {
        protected override string migrationId => "20200811085508_AddUserImageTable";

        public override int GetVersion()
        {
            return 4;
        }
    }
}
