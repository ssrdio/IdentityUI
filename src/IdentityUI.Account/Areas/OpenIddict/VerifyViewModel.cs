using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.OpenIddict
{
    public class VerifyViewModel
    {
        public string ApplicationName { get; set; }
        public string Error { get; set; }
        public string ErrorDescription { get; set; }
        public string Scope { get; set; }
        public string UserCode { get; set; }
    }
}
