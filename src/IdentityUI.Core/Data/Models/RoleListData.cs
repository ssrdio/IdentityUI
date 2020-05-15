using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Models
{
    public class RoleListData
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public RoleListData(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
