using OpenIddict.EntityFrameworkCore.Models;
using System;

namespace SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect
{
    public class ClientEntity : OpenIddictEntityFrameworkCoreApplication<string, ClientConsentEntity, ClientTokenEntity>, IIdentityUIEntity, ITimestampEntity, ISoftDelete
    {
        public DateTimeOffset? _CreatedDate { get; set; }
        public DateTimeOffset? _ModifiedDate { get; set; }
        public DateTimeOffset? _DeletedDate { get; set; }
    }
}
