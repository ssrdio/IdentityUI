using FluentValidation;
using SSRD.AdminUI.Template.Services;

namespace SSRD.IdentityUI.Core.Services.Auth.Login.Models
{
    public class LoginWith2faRequest : IReCaptchaRequest
    {
        public string Code { get; set; }
        public bool RememberMe { get; set; }
        public bool RememberMachine { get; set; }
        public string ReCaptchaToken { get; set; }
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
