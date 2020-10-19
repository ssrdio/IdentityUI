using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SSRD.IdentityUI.Core.DependencyInjection;
using SSRD.IdentityUI.Core.Infrastructure.Data;
using SSRD.IdentityUI.Core.Infrastructure.Data.Providers.SqlServer;
using SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.EntityFrameworkCore.SqlServer.DependencyInjection
{
    public static class IdentityUISqlServerExtensions
    {
        public static IdentityUIServicesBuilder UseSqlServer(this IdentityUIServicesBuilder builder)
        {
            builder.UseSqlServer(builder.DatabaseOptions.ConnectionString);

            return builder;
        }

        public static IdentityUIServicesBuilder UseSqlServer(this IdentityUIServicesBuilder builder, string connectionString)
        {
            builder.Services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseSqlServer(connectionString, b =>
                {
                    b.MigrationsAssembly(typeof(IdentityUISqlServerExtensions).Assembly.FullName);
                });
            });

            builder.Services.AddSingleton<IUpdateList, SqlServerUpdateList>();

            return builder;
        }
    }
}
