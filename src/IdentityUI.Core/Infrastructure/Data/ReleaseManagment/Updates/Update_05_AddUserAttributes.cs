using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment.Updates
{
    internal class Update_05_AddUserAttributes : AbstractUpdate
    {
        protected override string migrationId => "20200831195154_AddUserAttributes";

        public override int GetVersion()
        {
            return 5;
        }
    }
}
