using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.User.Models
{
    public class ExternalLoginRegisterRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    internal class ExternalLoginRegisterRequestValidator : AbstractValidator<ExternalLoginRegisterRequest>
    {
        public ExternalLoginRegisterRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty();

            RuleFor(x => x.LastName)
                .NotEmpty();

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
