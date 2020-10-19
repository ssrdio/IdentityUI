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

        public int GetPageStart(int pageLenght = Select2Constants.DEFAULT_PAGE_SIZE)
        {
            if(!Page.HasValue)
            {
                return -1;
            }

            return (Page.Value - 1) * pageLenght;
        }

        public int GetPageLenght(int pageLenght = Select2Constants.DEFAULT_PAGE_SIZE)
        {
            if (!Page.HasValue)
            {
                return -1;
            }

            return pageLenght;
        }

        public bool IsMore(int count, int pageLenght = Select2Constants.DEFAULT_PAGE_SIZE)
        {
            bool more = false;
            if (GetPageLenght(pageLenght) < 0)
            {
                more = false;
            }
            else if (GetPageStart(pageLenght) + pageLenght < count)
            {
                more = true;
            }

            return more;
        }
    }

    internal class Select2RequestValidator : AbstractValidatorWithNullCheck<Select2Request>
    {
        public Select2RequestValidator()
        {
            RuleFor(x => x.Page)
                .Must(x => x >= 0)
                .When(x => x.Page.HasValue);
        }
    }
}
