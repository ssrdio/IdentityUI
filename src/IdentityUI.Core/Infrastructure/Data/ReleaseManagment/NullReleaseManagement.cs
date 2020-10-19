using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment
{
    /// <summary>
    /// only used for InMemory database provider
    /// </summary>
    internal class NullReleaseManagement : IReleaseManagement
    {
        public void ApplayMigrations()
        {
        }
    }
}
