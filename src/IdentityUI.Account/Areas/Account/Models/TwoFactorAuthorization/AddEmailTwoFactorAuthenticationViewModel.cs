using FluentValidation;
using SSRD.AdminUI.Template.Models;
using System.ComponentModel;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Manage
{
    public class AddEmailTwoFactorAuthenticationViewModel
    {
        [DisplayName("Token")]
        public string Token { get; set; }

        public StatusAlertViewModel StatusAlert { get; set; }
    }
}
