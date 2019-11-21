using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.User
{
    public class UserRolesViewModel
    {
        [DisplayName("User id")]
        public string UserId { get; set; }
        [DisplayName("Username")]
        public string UserName { get; set; }

        public UserRolesViewModel(string userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }
    }
}
