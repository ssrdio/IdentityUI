using SSRD.AdminUI.Template.Models;
using System.ComponentModel;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Manage
{
    public class ProfileViewModel
    {
        [DisplayName("Username")]
        public string UserName { get; set; }
        [DisplayName("First name")]
        public string FirstName { get; set; }
        [DisplayName("Last name")]
        public string LastName { get; set; }
        [DisplayName("Phone number")]
        public string PhoneNumber { get; set; }
        public bool IsPhoneNumberConfirmed { get; set; }

        public StatusAlertViewModel StatusAlert { get; set; }

        public ProfileViewModel(string userName, string firstName, string lastName, string phoneNumber, bool isPhoneNumberConfirmed)
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            IsPhoneNumberConfirmed = isPhoneNumberConfirmed;
        }

        public ProfileViewModel(StatusAlertViewModel statusAlert)
        {
            StatusAlert = statusAlert;
        }
    }
}
