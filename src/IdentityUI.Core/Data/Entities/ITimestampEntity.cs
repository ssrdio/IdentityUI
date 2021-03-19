using System;

namespace SSRD.IdentityUI.Core.Data.Entities
{
    public interface ITimestampEntity
    {
        DateTimeOffset? _CreatedDate { get; set; }
        DateTimeOffset? _ModifiedDate { get; set; }
    }
}
