using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.User
{
    public class UserListViewModel
    {
        public string Id { get; set; }
        [DisplayName("Username")]
        public string UserName { get; set; }
        public string Email { get; set; }
        [DisplayName("First name")]
        public string FirstName { get; set; }
        [DisplayName("Last name")]
        public string LastName { get; set; }

        public UserListViewModel(string id, string userName, string email, string firstName, string lastName)
        {
            Id = id;
            UserName = userName;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
