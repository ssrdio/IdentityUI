using FluentValidation;
using SSRD.AdminUI.Template.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Auth.Credentials.Models
{
    public class RecoverPasswordRequest : IReCaptchaRequest
    {
        public string Email { get; set; }

        public string ReCaptchaToken { get; set; }
    }

    internal class RecoverPasswordValidator : AbstractValidator<RecoverPasswordRequest>
    {
        public RecoverPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
