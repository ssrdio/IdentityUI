using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.User.Models
{
    public class RegisterRequest : IUserAttributeRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public string PhoneNumber { get; set; }
        public string Username { get; set; }

        public IDictionary<string, string> Attributes { get; set; }
    }

    internal class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();

            RuleFor(x => x.FirstName);

            RuleFor(x => x.LastName);

            RuleFor(x => x.PhoneNumber);

            RuleFor(x => x.Username);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .Equal(x => x.Password).WithMessage("'ConfirmPassword' does not match 'Password'");
        }
    }
}
