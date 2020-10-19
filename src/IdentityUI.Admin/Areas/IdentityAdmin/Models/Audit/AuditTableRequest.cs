using FluentValidation;
using SSRD.Audit.Data;
using SSRD.CommonUtils.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Audit
{
    public class AuditTableRequest
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

    public class AuditTableRequestValidator : AbstractValidator<AuditTableRequest>
    {
        public AuditTableRequestValidator()
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
