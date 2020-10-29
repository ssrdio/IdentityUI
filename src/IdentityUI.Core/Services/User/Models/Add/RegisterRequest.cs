using FluentValidation;
using SSRD.IdentityUI.Core.Services.User.Models.Add;

namespace SSRD.IdentityUI.Core.Services.User.Models
{
    public class RegisterRequest : BaseRegisterRequest
    {
    }

    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
        }
    }
}
