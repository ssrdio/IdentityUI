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
    internal class ReleaseManagement
    {
        private readonly IdentityDbContext _context;

#if NET_CORE2
        private readonly IHostingEnvironment _hostingEnvironment;
#endif
#if NET_CORE3
        private readonly IWebHostEnvironment _hostingEnvironment;
#endif

        private readonly ILogger<ReleaseManagement> _logger;

        private readonly DatabaseOptions _databaseOptions;
#if NET_CORE2
        public ReleaseManagement(IdentityDbContext context, IHostingEnvironment hostingEnvironment, ILogger<ReleaseManagement> logger,
            IOptionsSnapshot<DatabaseOptions> databaseOptions)
#elif NET_CORE3
        public ReleaseManagement(IdentityDbContext context, IWebHostEnvironment hostingEnvironment, ILogger<ReleaseManagement> logger,
            IOptionsSnapshot<DatabaseOptions> databaseOptions)
#endif
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _databaseOptions = databaseOptions.Value;
        }

        /// <summary>
        /// Executes pending migrations and update procedures.
        /// </summary>
        public void ApplayMigrations()
        {
            if (_databaseOptions.Type == DatabaseTypes.InMemory)
            {
                return;
            }

            _logger.LogInformation("===== Release Management =====");

            try
            {
                _logger.LogInformation($"Hosting env: {_hostingEnvironment}");

                IRelationalDatabaseCreator relationalDatabaseCreator = _context.Database.GetService<IDatabaseCreator>() as IRelationalDatabaseCreator;

                if (!relationalDatabaseCreator.Exists())
                {
                    _logger.LogInformation($"Creating new Database");
                    relationalDatabaseCreator.Create();
                    _logger.LogInformation($"New Database was created");
                }

                List<IUpdate> updates = AllUpdates();
                _logger.LogInformation($"All updates(also already applied): {string.Join(", ", updates)}");

                List<string> applyedMigrations = _context.Database
                    .GetAppliedMigrations()
                    .ToList();

                foreach (var update in updates)
                {
                    if (update.ShouldExecute(applyedMigrations))
                    {
                        PerformUpdateInTransaction(update);
                    }
                }

                _logger.LogInformation("===== Release Management Finished =====");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during ReleaseManagment. {ex}");
                throw new Exception($"ReleaseManagment Error. {ex.Message}");
            }
        }
        

        private void PerformUpdateInTransaction(IUpdate update)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _logger.LogInformation($"===== Performing update: {update} =====");
                    _logger.LogInformation($"Before Schema Change: {update}");
                    update.BeforeSchemaChange(_context);
                    _logger.LogInformation($"Finished Before Schema Change: {update}");

                    _logger.LogInformation($"Schema Change: {update}");
                    update.SchemaChange(_context.Database);
                    _logger.LogInformation($"Finished Schema Change: {update}");

                    _logger.LogInformation($"After Schema Change: {update}");
                    update.AfterSchemaChange(_context);
                    _logger.LogInformation($"Finished After Schema Change: {update}");
                    _logger.LogInformation($"===== Finished update: {update} =====");

                    dbContextTransaction.Commit();
                }
                catch (Exception e)
                {
                    _logger.LogError($"Release update failed. {e}");
                    dbContextTransaction.Rollback();
                    throw e;
                }
            }
        }

        private List<IUpdate> AllUpdates()
        {
            var updates = new List<IUpdate>
            {
                new Update_01_InitialCreate(),
                new Update_02_AddPermissionsAddGroups(),
                new Update_03_UserActiveTwoFactorAuthenticationColumnAdded(),
                new Update_04_AddUserImageTable(),
            };
            return updates.OrderBy(m => m.GetVersion()).ToList();
        }
    }
}
