using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.OpenIddict
{
    public class AuthorizeViewModel
    {
        public string ApplicationName { get; set; }
        public List<string> Scopes { get; set; }
    }
}
