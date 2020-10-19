using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SSRD.IdentityUI.EntityFrameworkCore.Postgre.Updates
{
    public class Update_20191105162011_InitialCreate : PostgreUpdate
    {
        public override string MigrationId => "20191105162011_InitialCreate";
    }
}
