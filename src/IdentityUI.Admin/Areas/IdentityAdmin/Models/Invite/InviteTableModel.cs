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
        public string Role { get; set; }
        public string Group { get; set; }
        public string GroupRole { get; set; }
        public string ExpiresAt { get; set; }

        public InviteTableModel(string id, string email, string status, string role, string group, string groupRole, string expiresAt)
        {
            Id = id;
            Email = email;
            Status = status;
            Role = role;
            Group = group;
            GroupRole = groupRole;
            ExpiresAt = expiresAt;
        }
    }
}
