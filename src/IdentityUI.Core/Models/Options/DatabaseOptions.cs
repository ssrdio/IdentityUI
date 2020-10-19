using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Models.Options
{
    public class DatabaseOptions
    {
        public DatabaseTypes Type { get; set; }
        public string ConnectionString { get; set; }
    }

    public enum DatabaseTypes
    {
        PostgreSql,
        InMemory,
        SqlServer,
    }
}
