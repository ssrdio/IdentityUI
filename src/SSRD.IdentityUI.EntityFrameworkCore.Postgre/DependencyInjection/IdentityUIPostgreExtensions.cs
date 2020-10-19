using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SSRD.IdentityUI.Core.DependencyInjection;
using SSRD.IdentityUI.Core.Infrastructure.Data;
using SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment;
using SSRD.IdentityUI.Core.Models.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.EntityFrameworkCore.Postgre.DependencyInjection
{
    public static class IdentityUIPostgreExtensions
    {
        public static IdentityUIServicesBuilder UsePostgre(this IdentityUIServicesBuilder builder)
        {
            builder.UsePostgre(builder.DatabaseOptions.ConnectionString);

            return builder;
        }

        public static IdentityUIServicesBuilder UsePostgre(this IdentityUIServicesBuilder builder, string connectionString)
        {
            builder.Services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseNpgsql(connectionString, b =>
                {
                    b.MigrationsAssembly(typeof(IdentityUIPostgreExtensions).Assembly.FullName);
                });
            });

            builder.Services.AddSingleton<IUpdateList, PostgreUpdateList>();

            return builder;
        }
    }
}
