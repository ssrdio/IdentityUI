using SSRD.AdminUI.Template.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Models.Options
{
    public class IdentityUIOptions
    {
        public string BasePath { get; set; }

        public int MaxProfileImageSize { get; set; } = 200 * 1024;

        public EmailSenderOptions EmailSender { get; set; }
        public DatabaseOptions Database { get; set; }

        public ReCaptchaOptions ReCaptcha { get; set; }
    }
}
