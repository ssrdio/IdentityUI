using FluentValidation;
using SSRD.IdentityUI.Core.Services.User.Models.Add;

namespace SSRD.IdentityUI.Core.Services.User.Models
{
    public class AcceptInviteRequest : BaseRegisterRequest
    {
        public string Code { get; set; }
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
