using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models
{
    public class ClientScopeDetailsModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }

        public ClientScopeDetailsModel(string name, string displayName, string description)
        {
            Name = name;
            DisplayName = displayName;
            Description = description;
        }
    }
}
