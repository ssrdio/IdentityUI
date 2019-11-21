using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Entities
{
    public interface ISoftDelete
    {
        DateTimeOffset? _DeletedDate { get; set; }
    }
}
