using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.User
{
    public class UserSessionViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }

        public UserSessionViewModel(string userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }
    }
}
