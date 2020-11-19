using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Models.Options
{
    public class EmailSenderOptions
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        [Obsolete("Use SenderEmail insted")]
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderDisplayName { get; set; }
        public bool UseSSL { get; set; }

        public EmailSenderOptions()
        {

        }
    }
}
