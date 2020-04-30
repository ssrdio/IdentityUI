using SSRD.IdentityUI.Core.Data.Enums.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Entities
{
    public class EmailEntity : IBaseEntity
    {
        public long Id { get; private set; }

        public string Subject { get; private set; }
        public string Body { get; private set; }

        public EmailTypes Type { get; private set; }

        public DateTimeOffset? _CreatedDate { get; set; }
        public DateTimeOffset? _ModifiedDate { get; set; }

        public EmailEntity(string subject, string body, EmailTypes type)
        {
            Subject = subject;
            Body = body;
            Type = type;
        }

        public void Update(string subject, string body)
        {
            Subject = subject;
            Body = body;
        }
    }
}
