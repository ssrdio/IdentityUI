using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Invite
{
    public class InviteTableModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string ExpiresAt { get; set; }

        public InviteTableModel(string id, string email, string status, string expiresAt)
        {
            Id = id;
            Email = email;
            Status = status;
            ExpiresAt = expiresAt;
        }
    }
}
