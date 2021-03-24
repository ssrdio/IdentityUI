using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect.Models
{
    public class GenerateNewClientSecretModel
    {
        public string Secret { get; set; }

        public GenerateNewClientSecretModel()
        {
        }

        public GenerateNewClientSecretModel(string secret)
        {
            Secret = secret;
        }
    }
}
