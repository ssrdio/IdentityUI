using SSRD.AdminUI.Template.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Account
{
    public class ExternalLoginRegisterViewModel
    {
        public string Email { get; set; }
        [DisplayName("First name")]

        public string FirstName { get; set; }
        [DisplayName("Last name")]

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }
        public string Username { get; set; }

        public string ExternalLoginProviderName { get; set; }

        public string ReturnUrl { get; set; }

        public IDictionary<string, string> Attributes { get; set; }

        public StatusAlertViewModel StatusAlert { get; set; }

        public ExternalLoginRegisterViewModel(string externalLoginProviderName)
        {
            ExternalLoginProviderName = externalLoginProviderName;
        }

        public ExternalLoginRegisterViewModel(string email, string firstName, string lastName, string externalLoginProviderName, string returnUrl)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            ExternalLoginProviderName = externalLoginProviderName;
            ReturnUrl = returnUrl;
        }
    }
}
