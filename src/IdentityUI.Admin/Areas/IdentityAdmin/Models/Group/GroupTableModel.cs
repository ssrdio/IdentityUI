using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Group
{
    public class GroupTableModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public GroupTableModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
