using SSRD.AdminUI.Template.Models;
using System.Collections.Generic;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Manage
{
    public class TwoFactorRecoverCodesViewModel
    {
        public List<string> RecoverCodes { get; set; }

        public TwoFactorRecoverCodesViewModel(List<string> recoverCodes)
        {
            RecoverCodes = recoverCodes;
        }
    }
}
