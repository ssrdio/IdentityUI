using FluentValidation;
using SSRD.IdentityUI.Core.Helper;

namespace SSRD.IdentityUI.Core.Services.Group.Models
{
    public class UpdateGroupModel
    {
        public string GroupName { get; set; }
    }

    public class UpdateGroupModelValidator : AbstractValidatorWithNullCheck<UpdateGroupModel>
    {
        public UpdateGroupModelValidator()
        {
            RuleFor(x => x.GroupName)
                .NotEmpty();
        }
    }
}
