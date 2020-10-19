using FluentValidation;
using SSRD.Audit.Data;
using SSRD.CommonUtils.Specifications;
using SSRD.IdentityUI.Core.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Audit
{
    public class AuditTableRequest
    {
        public ActionTypes? ActionType { get; set; }
        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
        public OrderByTypes? OrderBy { get; set; }
    }

    public class AuditTableRequestValidator : AbstractValidatorWithNullCheck<AuditTableRequest>
    {
        public AuditTableRequestValidator()
        {
            RuleFor(x => x.ActionType)
                .IsInEnum()
                .When(x => x.ActionType.HasValue);

            RuleFor(x => x.OrderBy)
                .IsInEnum()
                .NotNull();
        }
    }
}
