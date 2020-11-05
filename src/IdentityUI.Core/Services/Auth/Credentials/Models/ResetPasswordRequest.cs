using FluentValidation;
using SSRD.AdminUI.Template.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Auth.Credentials.Models
{
    public class ResetPasswordRequest : IReCaptchaRequest
    {
        public string Code { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public string ReCaptchaToken { get; set; }
    }

    internal class ResetPasswordValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty(); // TO DO: change

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .Equal(x => x.Password).WithMessage("'ConfirmPassword' does not match 'Password'");
        }
    }
}
