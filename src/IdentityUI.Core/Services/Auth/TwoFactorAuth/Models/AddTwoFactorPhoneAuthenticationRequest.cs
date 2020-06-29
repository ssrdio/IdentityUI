using FluentValidation;

namespace SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth.Models
{
    public class AddTwoFactorPhoneAuthenticationRequest
    {
        public string Token { get; set; }
    }

    internal class AddTwoFactorPhoneAuthenticationRequestValidator : AbstractValidator<AddTwoFactorPhoneAuthenticationRequest>
    {
        public AddTwoFactorPhoneAuthenticationRequestValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty();
        }
    }
}
