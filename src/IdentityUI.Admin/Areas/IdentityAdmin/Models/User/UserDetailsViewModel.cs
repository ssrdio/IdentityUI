using SSRD.AdminUI.Template.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.User
{
    public class UserDetailsViewModel
    {
        public string Id { get; set; }
        [DisplayName("Username")]
        public string UserName { get; set; }
        public string Email { get; set; }
        [DisplayName("First name")]
        public string FirstName { get; set; }
        [DisplayName("Last name")]
        public string LastName { get; set; }
        [DisplayName("Email confirmed")]
        public bool EmailConfirmed { get; set; }
        [DisplayName("Phone number")]
        public string PhoneNumber { get; set; }
        [DisplayName("Phone number confirmed")]
        public bool PhoneNumberConfirmed { get; set; }
        [DisplayName("Two factor enabled")]
        public bool TwoFactorEnabled { get; set; }
        public bool Enabled { get; set; }
        public string LockoutEnd { get; set; }

        public bool UseEmailSender { get; set; }

        public StatusAlertViewModel StatusAlert { get; set; }

        public UserDetailsViewModel(string id, string userName, string email, string firstName, string lastName, bool emailConfirmed, string phoneNumber,
            bool phoneNumberConfirmed, bool twoFactorEnabled, bool enabled, string lockoutEnd)
        {
            Id = id;
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

        public UserDetailsViewModel(StatusAlertViewModel statusAlert)
        {
            StatusAlert = statusAlert;
        }
    }
}
