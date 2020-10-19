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
    internal class ReleaseManagement : IReleaseManagement
    {
        private readonly IdentityDbContext _context;

#if NET_CORE2
        private readonly IHostingEnvironment _hostingEnvironment;
#endif
#if NET_CORE3
        private readonly IWebHostEnvironment _hostingEnvironment;
#endif
        private readonly IUpdateList _updateList;

        private readonly ILogger<ReleaseManagement> _logger;
#if NET_CORE2
        public ReleaseManagement(IdentityDbContext context, IHostingEnvironment hostingEnvironment, IUpdateList updateList,
            ILogger<ReleaseManagement> logger)
#elif NET_CORE3
        public ReleaseManagement(IdentityDbContext context, IWebHostEnvironment hostingEnvironment, IUpdateList updateList,
            ILogger<ReleaseManagement> logger)
#endif
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _updateList = updateList;
            _logger = logger;
        }

        /// <summary>
        /// Executes pending migrations and update procedures.
        /// </summary>
        public void ApplayMigrations()
        {
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

                IEnumerable<IUpdate> updates = _updateList.Get();

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
                    update.SchemaChange(_context);
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
    }
}
