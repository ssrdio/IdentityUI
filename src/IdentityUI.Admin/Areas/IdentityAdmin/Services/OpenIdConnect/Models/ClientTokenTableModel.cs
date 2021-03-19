using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models
{
    public class ClientTokenTableModel
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? RedemptionDate { get; set; }

        public ClientTokenTableModel(
            string id,
            string status,
            string type,
            string subject,
            DateTime? createdDate,
            DateTime? expirationDate,
            DateTime? redemptionDate)
        {
            Id = id;
            Status = status;
            Type = type;
            Subject = subject;
            CreatedDate = createdDate;
            ExpirationDate = expirationDate;
            RedemptionDate = redemptionDate;
        }
    }
}
