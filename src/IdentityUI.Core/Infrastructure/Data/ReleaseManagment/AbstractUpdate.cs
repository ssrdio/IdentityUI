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
    public abstract class AbstractUpdate : IUpdate
    {
        public abstract string MigrationId { get; }

        protected AbstractUpdate()
        {
        }

        public virtual void BeforeSchemaChange(IdentityDbContext context)
        {
        }

        public virtual void SchemaChange(IdentityDbContext context)
        {
            ExecuteMigrationSql(context);
        }

        public virtual void AfterSchemaChange(IdentityDbContext context)
        {
        }

        public virtual bool ShouldExecute(List<string> applaydMigrations)
        {
            return !applaydMigrations.Contains(MigrationId);
        }

        protected abstract string GetScript();

        /// <summary>
        /// Executed migration SQL script located in Migrations/Scripts folder.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void ExecuteMigrationSql(IdentityDbContext context)
        {
            string sql = GetScript();

#if NET_CORE2
            context.Database.ExecuteSqlCommand(sql);
#endif
#if NET_CORE3
            context.Database.ExecuteSqlRaw(sql);
#endif

        }

    }
}
