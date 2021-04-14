using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Consent
{
    public class ConsentModel
    {
        public string Id { get; set; }
        public string Scopes { get; set; }
        public string Type { get; set; }
        public string Client { get; set; }

        public DateTime? CreatedDate { get; set; }

        public ConsentModel(string id, string scopes, string type, string client, DateTime? createdDate)
        {
            Id = id;
            Scopes = scopes;
            Type = type;
            Client = client;
            CreatedDate = createdDate;
        }
    }
}
