using FluentValidation;
using SSRD.AdminUI.Template.Services;
using System;

namespace SSRD.IdentityUI.Core.Services.Auth.Login.Models
{
    public class LoginWithRecoveryCodeRequest : IReCaptchaRequest
    {
        public string RecoveryCode { get; set; }
        public string ReCaptchaToken { get; set; }
    }

    internal class LoginWithRecoveryCodeRequestValidator : AbstractValidator<LoginWithRecoveryCodeRequest>
    {
        public LoginWithRecoveryCodeRequestValidator()
        {
            RuleFor(x => x.RecoveryCode)
                .NotEmpty();
        }
    }
}
