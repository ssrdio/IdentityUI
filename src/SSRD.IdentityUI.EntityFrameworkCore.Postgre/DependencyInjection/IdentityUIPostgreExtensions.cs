using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SSRD.IdentityUI.Core.DependencyInjection;
using SSRD.IdentityUI.Core.Infrastructure.Data;
using SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment;

namespace SSRD.IdentityUI.EntityFrameworkCore.Postgre.DependencyInjection
{
    public static class IdentityUIPostgreExtensions
    {
        public static IdentityUIServicesBuilder UsePostgre(this IdentityUIServicesBuilder builder)
        {
            builder.UsePostgre(builder.DatabaseOptions.ConnectionString, builder.DatabaseOptions.OpenIddictConnectionString);

            return builder;
        }

        public static IdentityUIServicesBuilder UsePostgre(this IdentityUIServicesBuilder builder, string identityUIConnectionString, string openIddictConnectionString)
        {
            builder.Services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseNpgsql(identityUIConnectionString, b =>
                {
                    b.MigrationsAssembly(typeof(IdentityUIPostgreExtensions).Assembly.FullName);
                });
            });

            builder.Services.AddSingleton<IUpdateList, PostgreUpdateList>();

            return builder;
        }
    }
}
