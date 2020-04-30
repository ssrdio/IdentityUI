using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Enums.Entity
{
    public enum EmailTypes
    {
        EmailConfirmation = 1,
        PasswordRecovery = 2,
        PasswordWasReset = 3,
        Invite = 4,
    }
}
