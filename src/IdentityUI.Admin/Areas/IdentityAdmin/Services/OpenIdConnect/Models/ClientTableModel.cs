using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect
{
    public class ClientTableModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public ClientTableModel(string id, string name, string type)
        {
            Id = id;
            Name = name;
            Type = type;
        }
    }
}
