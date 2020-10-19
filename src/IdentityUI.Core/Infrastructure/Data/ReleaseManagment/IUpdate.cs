using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment
{
    public interface IUpdate
    {
        string MigrationId { get; }

        /// <summary>
        /// Determines if update(and migration) will be run. 
        /// If not overwritten, it returns true for all pending transactions("__EFMigrationsHistory" table doesn't contain this update ID).
        /// Useful when additional conditions for execution are necessary, e.g.: its only production update
        /// </summary>
        /// <param name="applyedMigrations">List of pending migration IDs</param>
        /// <returns></returns>
        bool ShouldExecute(List<string> applyedMigrations);

        /// <summary>
        /// Performed before database schema change is applied.
        /// </summary>
        void BeforeSchemaChange(IdentityDbContext context);

        /// <summary>
        /// Performs database schema change, usually SQL read from Migrations/Scripts folder.
        /// </summary>
        void SchemaChange(IdentityDbContext context);

        /// <summary>
        /// Performed after database schema change is applied.
        /// </summary>
        void AfterSchemaChange(IdentityDbContext context);
    }
}
