using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.User.Models
{
    public class SetNewPasswordRequest
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class SetNewPasswordValidator : AbstractValidator<SetNewPasswordRequest>
    {
        public SetNewPasswordValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty();

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .Equal(x => x.Password).WithMessage("'ConfirmPassword' does not match 'Password'");
        }
    }
}
