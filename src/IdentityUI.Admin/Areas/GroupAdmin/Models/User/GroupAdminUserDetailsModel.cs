using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.User
{
    public class GroupAdminUserDetailsModel
    {
        public string UserId { get; set; }

        public long GroupUserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorAuthenticationEnabled { get; set; }
        public bool Enabled { get; set; }
        public string LockedOutTo { get; set; }

        public GroupAdminUserDetailsModel(
            string userId,
            long groupUserId,
            string username,
            string email,
            string firstName,
            string lastName,
            string phoneNumber,
            bool emailConfirmed,
            bool phoneNumberConfirmed,
            bool twoFactorAuthenticationEnabled,
            bool enabled,
            string lockedOutTo)
        {
            UserId = userId;
            GroupUserId = groupUserId;
            Username = username;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            EmailConfirmed = emailConfirmed;
            PhoneNumberConfirmed = phoneNumberConfirmed;
            TwoFactorAuthenticationEnabled = twoFactorAuthenticationEnabled;
            Enabled = enabled;
            LockedOutTo = lockedOutTo;
        }
    }
}
