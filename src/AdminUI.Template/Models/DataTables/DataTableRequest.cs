using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using SSRD.CommonUtils.Validation;

namespace SSRD.AdminUI.Template.Models.DataTables
{
    public class DataTableRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        [FromQuery(Name = "search[value]")]
        public string Search { get; set; }
    }
    public class DataTableRequestValidator : AbstractValidatorWithNullCheck<DataTableRequest>
    {
        public DataTableRequestValidator()
        {
            RuleFor(x => x.Draw)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Start)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Length)
                .GreaterThan(0);

            RuleSet("allowAll", () =>
            {
                RuleFor(x => x.Draw)
                    .GreaterThanOrEqualTo(0);

                RuleFor(x => x.Start)
                    .GreaterThanOrEqualTo(0);

                RuleFor(x => x.Length)
                    .GreaterThanOrEqualTo(-1)
                    .NotEqual(0);
            });
        }
    }
}
