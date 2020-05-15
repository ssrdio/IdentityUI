using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role
{
    public class UserTableModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        public string GroupName { get; set; }

        public UserTableModel(string id, string userName)
        {
            Id = id;
            UserName = userName;
        }

        public UserTableModel(string id, string userName, string groupName)
        {
            Id = id;
            UserName = userName;
            GroupName = groupName;
        }
    }
}
