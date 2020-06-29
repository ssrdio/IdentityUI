using SSRD.AdminUI.Template.Models;
using System.ComponentModel;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Manage
{
    public class AddPhoneTwoFactorAuthenticationViewModel
    {
        [DisplayName("Token")]
        public string Token { get; set; }

        [DisplayName("Phone number")]
        public string PhoneNumber { get; set; }

        public StatusAlertViewModel StatusAlert { get; set; }

        public AddPhoneTwoFactorAuthenticationViewModel()
        {
        }

        public AddPhoneTwoFactorAuthenticationViewModel(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }
    }
}
