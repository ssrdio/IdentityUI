using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Models
{
    public class AuditCommentData
    {
        public string Comment { get; set; }
        public DateTime Created { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }

        public AuditCommentData(string comment, DateTime created, string userId, string username)
        {
            Comment = comment;
            Created = created;
            UserId = userId;
            Username = username;
        }
    }
}
