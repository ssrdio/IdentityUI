using Microsoft.AspNetCore.Identity;
using SSRD.Audit.Attributes;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.User;
using SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth.Models;
using System;
using System.Collections.Generic;

namespace SSRD.IdentityUI.Core.Data.Entities.Identity
{
    public class AppUserEntity : IdentityUser, IBaseEntity, ISoftDelete
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

        public TwoFactorAuthenticationType TwoFactor { get; set; }

        public virtual UserImageEntity UserImage { get; set; }

        public ICollection<UserAttributeEntity> Attributes { get; set; }

        [AuditIgnore]
        public override string PasswordHash { get => base.PasswordHash; set => base.PasswordHash = value; }
        [AuditIgnore]
        public override string ConcurrencyStamp { get => base.ConcurrencyStamp; set => base.ConcurrencyStamp = value; }
        [AuditIgnore]
        public override string SecurityStamp { get => base.SecurityStamp; set => base.SecurityStamp = value; }

        /// <summary>
        /// This column does not exist in database. It is only used for login
        /// </summary>
        public string SessionCode { get; set; }

        /// <summary>
        /// This column does not exist in database. It is only used for login
        /// </summary>
        public string ImpersonatorId { get; set; }
        public DateTimeOffset? _DeletedDate { get; set; }

        public AppUserEntity()
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

        public AppUserEntity(
            string userName,
            string email,
            string firstName,
            string lastName,
            bool emailConfirmed,
            bool enabled,
            string phoneNumber,
            List<UserAttributeEntity> attributes)
        {
            UserName = userName;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            EmailConfirmed = emailConfirmed;
            Enabled = enabled;
            PhoneNumber = phoneNumber;
            Attributes = attributes;
        }

        [Obsolete("Use ICanLoginService")]
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
