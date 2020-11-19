using FluentValidation;
using System;

namespace SSRD.AdminUI.Template.Models
{
    public class TimeRangeRequest
    {
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }

    public class TimeRangeRequestValidator : AbstractValidatorWithNullCheck<TimeRangeRequest>
    {
        public TimeRangeRequestValidator()
        {
            RuleFor(x => x.From)
                .LessThanOrEqualTo(x => x.To);

            RuleFor(x => x.To)
                .GreaterThanOrEqualTo(x => x.From);
        }
    }
}
