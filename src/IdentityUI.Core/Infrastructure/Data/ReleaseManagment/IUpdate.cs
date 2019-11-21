using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment
{
    internal interface IUpdate
    {
        /// <summary>
        /// Return next ascending natural number, e.g. 1, 2, 3. Used for sorting update execution order.
        /// </summary>
        /// <returns>Previous update + 1</returns>
        int GetVersion();

        /// <summary>
        /// Deterimes if update(and migration) will be run. 
        /// If not overriten, it returnes true for all pending transactions("__EFMigrationsHistory" table doesn't contain this update ID).
        /// Useful when additional conditions for execution are neccessary, e.g.: its only production update
        /// </summary>
        /// <param name="applyedMigrations">List of pending migration IDs</param>
        /// <returns></returns>
        bool ShouldExecute(List<string> applyedMigrations);

        /// <summary>
        /// Performed before database schema change is applied.
        /// </summary>
        void BeforeSchemaChange();

        /// <summary>
        /// Performs database schema change, usually SQL read from Migrations/Scripts folder.
        /// </summary>
        /// <param name="database"></param>
        void SchemaChange(DatabaseFacade database);

        /// <summary>
        /// Performed after database schema change is applied.
        /// </summary>
        void AfterSchemaChange();
    }
}
