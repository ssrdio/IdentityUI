using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Group
{
    public class GroupAttributeTableModel
    {
        public long Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public GroupAttributeTableModel(long id, string key, string value)
        {
            Id = id;
            Key = key;
            Value = value;
        }
    }
}
