using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable
{
    [Obsolete("Use SSRD.AdminUI.Template.Models.DataTables")]
    public class DataTableRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        [FromQuery(Name = "search[value]")]
        public string Search { get; set; }
    }
    public class DataTableValidator : AbstractValidator<DataTableRequest>
    {
        public DataTableValidator()
        {
            RuleFor(x => x.Draw)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Start)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Length)
                .GreaterThan(0);
        }
    }
}
