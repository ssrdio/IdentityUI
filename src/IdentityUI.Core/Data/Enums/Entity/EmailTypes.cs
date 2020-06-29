using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Enums.Entity
{
    public enum EmailTypes
    {
        [Description("Email confirmation")]
        EmailConfirmation = 1,

        [Description("Password recovery")]
        PasswordRecovery = 2,

        [Description("Password was reset")]
        PasswordWasReset = 3,

        [Description("Invite")]
        Invite = 4,

        [Description("Two factor authentication token")]
        TwoFactorAuthenticationToken = 20,
    }
}
