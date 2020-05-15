using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.User.Models
{
    public class AcceptInviteRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    internal class AcceptInviteRequestValidator : AbstractValidator<AcceptInviteRequest>
    {
        public AcceptInviteRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty();

            RuleFor(x => x.LastName)
                .NotEmpty();

            RuleFor(x => x.Password)
                .NotEmpty();

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .Equal(x => x.Password).WithMessage("'ConfirmPassword' does not match 'Password'");

            RuleFor(x => x.Code)
                .NotEmpty();
        }
    }
}
