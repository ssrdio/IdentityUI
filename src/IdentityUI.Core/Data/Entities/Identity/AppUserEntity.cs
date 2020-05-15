using Microsoft.AspNetCore.Identity;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Data.Entities.Identity
{
    public class AppUserEntity : IdentityUser, IBaseEntity
    {
        public DateTimeOffset? _CreatedDate { get; set; }
        public DateTimeOffset? _ModifiedDate { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Enabled { get; set; }

        public virtual ICollection<UserClaimEntity> Claims { get; set; }
        public virtual ICollection<UserLoginEntity> Logins { get; set; }
        public virtual ICollection<UserTokenEntity> Tokens { get; set; }
        public virtual ICollection<UserRoleEntity> UserRoles { get; set; }
        public virtual ICollection<SessionEntity> Sessions { get; set; }

        public virtual ICollection<GroupUserEntity> Groups { get; set; }

        /// <summary>
        /// This column does not exist in database. It is only used for login
        /// </summary>
        public string SessionCode { get; set; }

        protected AppUserEntity()
        {
        }

        public AppUserEntity(string userName, string email, string firstName, string lastName, bool emailConfirmed, bool enabled)
        {
            UserName = userName;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            EmailConfirmed = emailConfirmed;
            Enabled = enabled;
        }

        public bool CanLogin()
        {
            if(!Enabled)
            {
                return false;
            }

            return true;
        }
    }
}
