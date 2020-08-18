using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Auth.Login.Models
{
    public class ExternalLoginRequest
    {
        public string Provider { get; set; }
    }

    internal class ExternalLoginRequestValidator : AbstractValidator<ExternalLoginRequest>
    {
        public ExternalLoginRequestValidator()
        {
            RuleFor(x => x.Provider)
                .NotEmpty();
        }
    }
}
