using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Data.Entities.Identity
{
    public class UserClaimEntity : IdentityUserClaim<string>, IBaseEntity
    {
        public DateTimeOffset? _CreatedDate { get; set; }
        public DateTimeOffset? _ModifiedDate { get; set; }

        public virtual AppUserEntity User { get; set; }

        public UserClaimEntity()
        {
        }
    }
}
