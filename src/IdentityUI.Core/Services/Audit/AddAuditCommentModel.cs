using FluentValidation;
using SSRD.CommonUtils.Validation;

namespace SSRD.IdentityUI.Core.Services.Audit
{
    public class AddAuditCommentModel
    {
        public string Comment { get; set; }
    }

    public class AddAuditCommentModelValidator : AbstractValidatorWithNullCheck<AddAuditCommentModel>
    {
        public AddAuditCommentModelValidator()
        {
            RuleFor(x => x.Comment)
                .NotEmpty();
        }
    }
}
