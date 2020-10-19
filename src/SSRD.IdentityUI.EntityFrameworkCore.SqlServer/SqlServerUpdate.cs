using SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Providers.SqlServer
{
    internal abstract class SqlServerUpdate : AbstractUpdate
    {
        /// <summary>
        /// 0 = assembly name
        /// 1 = migration id
        /// </summary>
        public const string MIGRATIONS_PATH = "{0}.Scripts.Migrations.{1}.sql";

        protected override string GetScript()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(SqlServerUpdate));

            string sql = null;

            using (Stream resource = assembly.GetManifestResourceStream(string.Format(MIGRATIONS_PATH, assembly.GetName().Name, MigrationId)))
            {
                using (StreamReader reader = new StreamReader(resource))
                {
                    sql = reader.ReadToEnd();
                }
            }

            return sql;
        }
    }
}
