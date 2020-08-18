using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Auth.Credentials.Models
{
    public class CreatePasswordRequest
    {
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }

    public class CreatePasswordValidator : AbstractValidator<CreatePasswordRequest>
    {
        public CreatePasswordValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty();

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty()
                .Equal(x => x.NewPassword).WithMessage("'ConfirmNewPassword' does not match 'NewPassword'");
        }
    }
}
