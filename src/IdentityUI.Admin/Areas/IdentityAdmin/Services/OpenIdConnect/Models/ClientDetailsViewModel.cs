using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models
{
    public class ClientDetailsViewModel
    {
        public ClientMenuViewModel ClientMenu { get; set; }
        public List<string> Endpoints { get; set; }
        public List<string> GrantTypes { get; set; }
        public List<string> ResponseTypes { get; set; }

        public ClientDetailsViewModel(ClientMenuViewModel clientMenu, List<string> endpoints, List<string> grantTypes, List<string> responseTypes)
        {
            ClientMenu = clientMenu;
            Endpoints = endpoints;
            GrantTypes = grantTypes;
            ResponseTypes = responseTypes;
        }
    }
}
