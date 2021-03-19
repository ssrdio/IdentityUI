using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models
{
    public class ClientSecretModel
    {
        public bool IsSecretSet { get; set; }

        public ClientSecretModel(bool isSecretSet)
        {
            IsSecretSet = isSecretSet;
        }
    }
}
