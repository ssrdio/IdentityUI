using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.AdminUI.Template.Models.Select2
{
    public class Select2Request
    {
        public string Term { get; set; }
        public string Q { get; set; }
        public string _Type { get; set; }
        public int? Page { get; set; }
    }

    internal class Select2RequestValidator : AbstractValidator<Select2Request>
    {
        public Select2RequestValidator()
        {
            RuleFor(x => x.Page)
                .Must(x => x >= 0)
                .When(x => x.Page.HasValue);
        }
    }
}
