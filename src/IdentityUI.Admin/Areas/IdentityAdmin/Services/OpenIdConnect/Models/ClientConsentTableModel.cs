using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models
{
    public class ClientConsentTableModel
    {
        public string Id { get; set; }
        public string Scopes { get; set; }
        public string Status { get; set; }
        public string Subject { get; set; }
        public string Type { get; set; }

        public DateTime? CreatedDate { get; set; }

        public ClientConsentTableModel(string id, string scopes, string status, string subject, string type, DateTime? createdDate)
        {
            Id = id;
            Scopes = scopes;
            Status = status;
            Subject = subject;
            Type = type;
            CreatedDate = createdDate;
        }
    }
}
