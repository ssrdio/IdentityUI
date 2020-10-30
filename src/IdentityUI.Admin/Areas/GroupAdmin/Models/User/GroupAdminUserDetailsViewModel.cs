using SSRD.AdminUI.Template.Models;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.User
{
    public class GroupAdminUserDetailsViewModel
    {
        public long GroupUserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool Enabled { get; set; }
        public string LockoutEnd { get; set; }

        public bool UseEmailSender { get; set; }

        public StatusAlertViewModel StatusAlert { get; set; }

        public GroupAdminUserDetailsViewModel(
            long groupUserId,
            string userName,
            string email,
            string firstName,
            string lastName,
            bool emailConfirmed,
            string phoneNumber,
            bool phoneNumberConfirmed,
            bool twoFactorEnabled,
            bool enabled,
            string lockoutEnd)
        {
            GroupUserId = groupUserId;
            UserName = userName;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            EmailConfirmed = emailConfirmed;
            PhoneNumber = phoneNumber;
            PhoneNumberConfirmed = phoneNumberConfirmed;
            TwoFactorEnabled = twoFactorEnabled;
            Enabled = enabled;
            LockoutEnd = lockoutEnd;
        }

        public GroupAdminUserDetailsViewModel(StatusAlertViewModel statusAlert)
        {
            StatusAlert = statusAlert;
        }
    }
}
