using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Group
{
    public class GroupAttributeTableModel
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public GroupAttributeTableModel(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
