using FluentValidation;
using SSRD.AdminUI.Template.Services;

namespace SSRD.IdentityUI.Core.Services.User.Models
{
    public class ExternalLoginRegisterRequest : BaseRegisterRequest, IReCaptchaRequest
    {
#if NET_CORE2
        [Newtonsoft.Json.JsonIgnore]
#elif NET_CORE3
        [System.Text.Json.Serialization.JsonIgnore]
#endif
        public new string Password { get; set; }
#if NET_CORE2
        [Newtonsoft.Json.JsonIgnore]
#elif NET_CORE3
        [System.Text.Json.Serialization.JsonIgnore]
#endif
        public new string ConfirmPassword { get; set; }

        public string ReCaptchaToken { get; set; }
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
