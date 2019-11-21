using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Auth.Login.Models
{
    public class LoginWith2faRequest
    {
        public string Code { get; set; }
        public bool RememberMe { get; set; }
        public bool RememberMachine { get; set; }
    }

    internal class LoginWith2faRequestValidator : AbstractValidator<LoginWith2faRequest>
    {
        public LoginWith2faRequestValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(7);
        }
    }
}
