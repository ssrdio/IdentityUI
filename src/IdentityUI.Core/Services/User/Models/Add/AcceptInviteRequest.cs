using FluentValidation;
using SSRD.AdminUI.Template.Services;

namespace SSRD.IdentityUI.Core.Services.User.Models
{
    public class AcceptInviteRequest : BaseRegisterRequest, IReCaptchaRequest
    {
        public string Code { get; set; }

        public string ReCaptchaToken { get; set; }
    }

    internal class AcceptInviteRequestValidator : AbstractValidator<AcceptInviteRequest>
    {
        public AcceptInviteRequestValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty();
        }
    }
}
