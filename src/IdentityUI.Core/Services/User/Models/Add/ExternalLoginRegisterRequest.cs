using FluentValidation;
using SSRD.IdentityUI.Core.Services.User.Models.Add;

namespace SSRD.IdentityUI.Core.Services.User.Models
{
    public class ExternalLoginRegisterRequest : BaseRegisterRequest
    {
        [Newtonsoft.Json.JsonIgnore]
#if NET_CORE3
        [System.Text.Json.Serialization.JsonIgnore]
#endif
        public new string Password { get; set; }

        [Newtonsoft.Json.JsonIgnore]
#if NET_CORE3
        [System.Text.Json.Serialization.JsonIgnore]
#endif
        public new string ConfirmPassword { get; set; }
    }

    internal class ExternalLoginRegisterRequestValidator : AbstractValidator<ExternalLoginRegisterRequest>
    {
        public ExternalLoginRegisterRequestValidator()
        {
            RuleFor(x => x.Password)
                .Null();

            RuleFor(x => x.ConfirmPassword)
                .Null();
        }
    }
}
