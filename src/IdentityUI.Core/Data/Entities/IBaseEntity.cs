using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Entities
{
    public interface IBaseEntity
    {
        DateTimeOffset? _CreatedDate { get; set; }
        DateTimeOffset? _ModifiedDate { get; set; }
    }
}
