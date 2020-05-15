using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.AdminUI.Template.Models.Select2
{
    public class Select2Result<T>
    {
        public List<T> Results { get; set; }
        public Select2Pagination Pagination { get; set; }

        public Select2Result(List<T> results, bool pagination)
        {
            Results = results;

            Pagination = new Select2Pagination(
                more: pagination);
        }

        public Select2Result(List<T> results)
        {
            Results = results;

            Pagination = new Select2Pagination(
                more: false);
        }

        public class Select2Pagination
        {
            public bool More { get; set; }

            public Select2Pagination(bool more)
            {
                More = more;
            }
        }
    }
}
