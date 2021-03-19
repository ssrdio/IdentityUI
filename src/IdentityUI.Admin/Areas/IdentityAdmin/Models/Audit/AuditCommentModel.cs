using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Audit
{
    public class AuditCommentModel
    {
        public string Comment { get; set; }
        public string User { get; set; }
        public DateTime Created { get; set; }

        public AuditCommentModel(string comment, string user, DateTime created)
        {
            Comment = comment;
            User = user;
            Created = created;
        }
    }
}
