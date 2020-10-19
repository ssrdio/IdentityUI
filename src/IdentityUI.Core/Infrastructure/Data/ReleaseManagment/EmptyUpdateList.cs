using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment
{
    /// <summary>
    /// Only used for InMemory database provider
    /// </summary>
    class EmptyUpdateList : IUpdateList
    {
        public IEnumerable<IUpdate> Get()
        {
            return new List<IUpdate>();
        }
    }
}
