using Microsoft.EntityFrameworkCore.Infrastructure;
using SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment;
using SSRD.IdentityUI.EntityFrameworkCore.Postgre;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.EntityFrameworkCore.Postgre.Updates
{
    internal class Update_20200512083339_AddPermissionsAddGroups : PostgreUpdate
    {
        public override string MigrationId => "20200512083339_AddPermissionsAddGroups";
    }
}
