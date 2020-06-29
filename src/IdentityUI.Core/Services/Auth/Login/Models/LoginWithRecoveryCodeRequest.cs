using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Auth.Login.Models
{
    public class LoginWithRecoveryCodeRequest
    {
        public string RecoveryCode { get; set; }
    }

    internal class LoginWithRecoveryCodeRequestValidator : AbstractValidator<LoginWithRecoveryCodeRequest>
    {
        public LoginWithRecoveryCodeRequestValidator()
        {
            RuleFor(x => x.RecoveryCode)
                .NotEmpty();
        }
    }
}
