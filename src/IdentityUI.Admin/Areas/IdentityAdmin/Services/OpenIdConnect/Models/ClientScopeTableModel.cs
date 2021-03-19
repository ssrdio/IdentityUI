using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models
{
    public class ClientScopeTableModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ClientScopeTableModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
