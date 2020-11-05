using FluentValidation;
using SSRD.AdminUI.Template.Services;
using SSRD.IdentityUI.Core.Services.User.Models.Add;

namespace SSRD.IdentityUI.Core.Services.User.Models
{
    public class RegisterRequest : BaseRegisterRequest, IReCaptchaRequest
    {
        public string ReCaptchaToken { get; set; }
    }

    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
        }
    }
}
