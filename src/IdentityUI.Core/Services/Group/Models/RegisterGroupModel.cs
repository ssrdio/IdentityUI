using FluentValidation;
using SSRD.AdminUI.Template.Services;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Services.User.Models;

namespace SSRD.IdentityUI.Core.Services.Group.Models
{
    public class RegisterGroupModel : IReCaptchaRequest
    {
        public string GroupName { get; set; }

        public GroupBaseUserRegisterRequest BaseUser { get; set; }

        public string ReCaptchaToken { get; set; }
    }

    public class RegisterGroupModelValidator : AbstractValidatorWithNullCheck<RegisterGroupModel>
    {
        public RegisterGroupModelValidator()
        {
            RuleFor(x => x.GroupName)
                .NotEmpty();

            RuleFor(x => x.BaseUser)
                .NotNull();
        }
    }
}
