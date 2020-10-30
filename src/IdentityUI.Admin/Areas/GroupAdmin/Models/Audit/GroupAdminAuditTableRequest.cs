using FluentValidation;
using SSRD.AdminUI.Template.Models;
using SSRD.Audit.Data;
using SSRD.CommonUtils.Specifications;
using System;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Audit
{
    public class GroupAdminAuditTableRequest
    {
        public ActionTypes? ActionType { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public OrderByTypes? OrderBy { get; set; }

        public string ObjectType { get; set; }
        public string ObjectIdentifier { get; set; }

        public SubjectTypes? SubjectType { get; set; }
        public string SubjectIdentifier { get; set; }

        public string ResourceName { get; set; }
    }

    public class GroupAdminAuditTableRequestValidator : AbstractValidatorWithNullCheck<GroupAdminAuditTableRequest>
    {
        public GroupAdminAuditTableRequestValidator()
        {
            RuleFor(x => x.OrderBy)
                .NotNull()
                .IsInEnum();

            RuleFor(x => x.ActionType)
                .IsInEnum()
                .When(x => x.ActionType.HasValue);

            RuleFor(x => x.SubjectType)
                .IsInEnum()
                .When(x => x.SubjectType.HasValue);
        }
    }
}
