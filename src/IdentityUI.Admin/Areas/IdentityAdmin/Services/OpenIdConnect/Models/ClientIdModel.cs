using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models
{
    public class ClientIdModel
    {
        public string ClientId { get; set; }

        public ClientIdModel(string clientId)
        {
            ClientId = clientId;
        }
    }
}
