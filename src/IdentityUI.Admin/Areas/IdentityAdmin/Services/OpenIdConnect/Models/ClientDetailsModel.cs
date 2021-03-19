using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models
{
    public class ClientDetailsModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public IList<string> RedirectUrls { get; set; }
        public IList<string> PostLogoutUrls { get; set; }

        public IList<string> Endpoints { get; set; }
        public IList<string> GrantTypes { get; set; }
        public IList<string> ResponseTypes { get; set; }

        public bool RequireConsent { get; set; }
        public bool RequirePkce { get; set; }

        public ClientDetailsModel(
            string id,
            string name,
            IList<string> redirectUrls,
            IList<string> postLogoutUrls,
            IList<string> endpoints,
            IList<string> grantTypes,
            IList<string> responseTypes,
            bool requireConsent,
            bool requirePkce)
        {
            Id = id;
            Name = name;
            RedirectUrls = redirectUrls;
            PostLogoutUrls = postLogoutUrls;
            Endpoints = endpoints;
            GrantTypes = grantTypes;
            ResponseTypes = responseTypes;
            RequireConsent = requireConsent;
            RequirePkce = requirePkce;
        }
    }
}
