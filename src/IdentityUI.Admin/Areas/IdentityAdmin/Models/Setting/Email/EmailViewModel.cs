using SSRD.AdminUI.Template.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Setting.Email
{
    public class EmailViewModel
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public string Email { get; set; }
        public bool UseEmailSender { get; set; }

        public StatusAlertViewModel StatusAlert { get; set; }

        public EmailViewModel(long id, string type, string subject, string body)
        {
            Id = id;
            Type = type;
            Subject = subject;
            Body = body;
        }
    }
}
