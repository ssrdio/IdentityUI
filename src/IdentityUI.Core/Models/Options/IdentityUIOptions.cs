using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Models.Options
{
    public class IdentityUIOptions
    {
        public string BasePath { get; set; }

        public EmailSenderOptions EmailSender { get; set; }
        public DatabaseOptions Database { get; set; }
    }
}
