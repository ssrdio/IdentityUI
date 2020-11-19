using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Enums.Entity
{
    public enum SessionEndTypes
    {
        Logout = 1,
        Expired = 2,

        TwoFactorLogin = 5,
        InvlidTwoFactorLogin = 6,

        AdminLogout = 10,
        SecurityCodeChange = 11,

        InvalidLogin = 20,

        ImpersonationLogout = 30,

        AffterLoginFilterFailure = 40,
    }
}
