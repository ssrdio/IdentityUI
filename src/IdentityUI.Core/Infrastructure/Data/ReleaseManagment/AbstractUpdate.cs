using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment
{
    internal abstract class AbstractUpdate : IUpdate
    {
        private const string SCRIPTS_PATH = "Infrastructure.Data.Scripts.Migrations";

        protected abstract string migrationId { get; }

        public abstract int GetVersion();
        public abstract void BeforeSchemaChange();
        public abstract void SchemaChange(DatabaseFacade database);
        public abstract void AfterSchemaChange();

        public bool ShouldExecute(List<string> applaydMigrations)
        {
            return !applaydMigrations.Contains(migrationId);
        }

        protected AbstractUpdate()
        {
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }

        protected bool WasMigrationPerformed(string migrationId)
        {
            return false;
        }

        /// <summary>
        /// Executed migration SQL script located in Migrations/Scripts folder.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="migrationId">Update ID</param>
        protected void ExecuteMigrationSql(DatabaseFacade database, string migrationId)
        {
            Assembly assembly = typeof(AbstractUpdate).GetTypeInfo().Assembly;

            string sql;

            using (Stream resource = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{SCRIPTS_PATH}.{migrationId}.sql"))
            {
                using(StreamReader reader = new StreamReader(resource))
                {
                    sql = reader.ReadToEnd();
                }
            }

#if NET_CORE2
            database.ExecuteSqlCommand(sql);
#endif
#if NET_CORE3
            database.ExecuteSqlRaw(sql);
#endif

        }

    }
}
