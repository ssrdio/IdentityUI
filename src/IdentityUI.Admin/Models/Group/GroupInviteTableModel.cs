using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Models.Group
{
    public class GroupInviteTableModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string GroupRole { get; set; }
        public string Status { get; set; }
        public string ExpiresAt { get; set; }

        public GroupInviteTableModel(string id, string email, string groupRole, string status, string expiresAt)
        {
            Id = id;
            Email = email;
            GroupRole = groupRole;
            Status = status;
            ExpiresAt = expiresAt;
        }
    }
}
