using SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment.Updates;
using SSRD.IdentityUI.Core.Models.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment
{
    internal static class ReleaseManagement
    {
        /// <summary>
        /// Executes pending migrations and update procedures.
        /// </summary>
        public static void ApplyIdentityMigrations(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                ILoggerFactory loggerFactory = serviceScope.ServiceProvider.GetService<ILoggerFactory>();
                ILogger logger = loggerFactory.CreateLogger(typeof(ReleaseManagement));

                IOptionsSnapshot<DatabaseOptions> databaseOptions = serviceScope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DatabaseOptions>>();
                if(databaseOptions.Value.Type == DatabaseTypes.InMemory)
                {
                    return;
                }

                logger.LogInformation("===== Release Management =====");

                try
                {
#if NET_CORE2
                    IHostingEnvironment hostingEnvironment = serviceScope.ServiceProvider.GetRequiredService<IHostingEnvironment>();
#endif
#if NET_CORE3
                    IWebHostEnvironment hostingEnvironment = serviceScope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
#endif
                    IConfiguration configuration = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
                    IdentityDbContext context = serviceScope.ServiceProvider.GetRequiredService<IdentityDbContext>();
                    IRelationalDatabaseCreator databaseCreator = context.Database.GetService<IDatabaseCreator>() as IRelationalDatabaseCreator;


                    logger.LogInformation($"Hosting env: {hostingEnvironment}");

                    if(!databaseCreator.Exists())
                    {
                        logger.LogInformation($"Creating new Database");

                        databaseCreator.Create();

                        logger.LogInformation($"New Database was created");
                    }

                    List<IUpdate> updates = AllUpdates();
                    logger.LogInformation($"All updates(also already applied): {string.Join(", ", updates)}");

                    List<string> applyedMigrations = context.Database
                        .GetAppliedMigrations()
                        .ToList();

                    foreach (var update in updates)
                    {
                        if (update.ShouldExecute(applyedMigrations))
                        {
                            PerformUpdateInTransaction(context, update, logger);
                        }
                    }

                    logger.LogInformation("===== Release Management Finished =====");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error during ReleaseManagment. {ex}");
                    throw new Exception($"ReleaseManagment Error. {ex.Message}");
                }
            }

            return;
        }

        private static void PerformUpdateInTransaction(IdentityDbContext context, IUpdate update, ILogger logger)
        {
            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    logger.LogInformation($"===== Performing update: {update} =====");
                    logger.LogInformation($"Before Schema Change: {update}");
                    update.BeforeSchemaChange();
                    logger.LogInformation($"Finished Before Schema Change: {update}");

                    logger.LogInformation($"Schema Change: {update}");
                    update.SchemaChange(context.Database);
                    logger.LogInformation($"Finished Schema Change: {update}");

                    logger.LogInformation($"After Schema Change: {update}");
                    update.AfterSchemaChange();
                    logger.LogInformation($"Finished After Schema Change: {update}");
                    logger.LogInformation($"===== Finished update: {update} =====");

                    dbContextTransaction.Commit();
                }
                catch (Exception e)
                {
                    logger.LogError($"Release update failed. {e}");
                    dbContextTransaction.Rollback();
                    throw e;
                }
            }
        }

        private static List<IUpdate> AllUpdates()
        {
            var updates = new List<IUpdate>
            {
                new Update_01_InitialCreate(),
            };
            return updates.OrderBy(m => m.GetVersion()).ToList();
        }
    }
}
