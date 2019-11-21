using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth.Models
{
    public class AddTwoFactorAuthenticatorRequest
    {
        public string VereficationCode { get; set; }
    }

    internal class AddTwoFactorAuthicatorValidator : AbstractValidator<AddTwoFactorAuthenticatorRequest>
    {
        public AddTwoFactorAuthicatorValidator()
        {
            RuleFor(x => x.VereficationCode)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(7);
        }
    }
}
