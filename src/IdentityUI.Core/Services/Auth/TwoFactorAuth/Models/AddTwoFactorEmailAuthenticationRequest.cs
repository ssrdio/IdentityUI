using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth.Models
{
    public class AddTwoFactorEmailAuthenticationRequest
    {
        public string Token { get; set; }
    }

    internal class AddTwoFactorEmailAuthenticationRequestValidator : AbstractValidator<AddTwoFactorEmailAuthenticationRequest>
    {
        public AddTwoFactorEmailAuthenticationRequestValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty();
        }
    }
}
