using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.User
{
    public class UserAttributeTableModel
    {
        public long Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public UserAttributeTableModel(long id, string key, string value)
        {
            Id = id;
            Key = key;
            Value = value;
        }
    }
}
