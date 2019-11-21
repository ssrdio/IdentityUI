using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [DisplayName("Username")]
        public string UserName { get; set; }
        public string Email { get; set; }

        public UserViewModel(string id, string userName, string email)
        {
            Id = id;
            UserName = userName;
            Email = email;
        }
    }
}
