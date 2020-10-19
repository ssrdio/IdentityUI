using Microsoft.AspNetCore.Mvc;
using FluentValidation;

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

            //TODO: how to handle select all
            RuleFor(x => x.Length)
                .GreaterThan(0);
        }
    }
}
