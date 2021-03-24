using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Models.Seed
{
    public class ClientSeedModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public bool RequireConsent { get; set; }
        public bool RequirePkce { get; set; }

        public List<string> Scopes { get; set; }
        public List<string> RedirectUrls { get; set; }
        public List<string> PostLogoutUrls { get; set; }
        public List<string> Endpoints { get; set; }
        public List<string> GrantTypes { get; set; }
        public List<string> ResponseTypes { get; set; }

        public ClientSeedModel()
        {
        }

        public ClientSeedModel(
            string name,
            string description,
            string clientId,
            string secret,
            bool requireConsent,
            bool requirePkce,
            List<string> scopes,
            List<string> redirectUrls,
            List<string> postLogoutUrls,
            List<string> endpoints,
            List<string> grantTypes,
            List<string> responseTypes)
        {
            Name = name;
            Description = description;
            ClientId = clientId;
            Secret = secret;
            RequireConsent = requireConsent;
            RequirePkce = requirePkce;
            Scopes = scopes;
            RedirectUrls = redirectUrls;
            PostLogoutUrls = postLogoutUrls;
            Endpoints = endpoints;
            GrantTypes = grantTypes;
            ResponseTypes = responseTypes;
        }
    }
}
