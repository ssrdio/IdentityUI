using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role
{
    public class RoleListViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public RoleListViewModel(string id, string name, string type)
        {
            Id = id;
            Name = name;
            Type = type;
        }
    }
}
