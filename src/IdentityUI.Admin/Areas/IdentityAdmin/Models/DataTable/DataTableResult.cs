using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable
{
    [Obsolete("Use SSRD.AdminUI.Template.Models.DataTables")]
    public class DataTableResult<T>
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public string Error { get; set; }

        public List<T> Data { get; set; }

        public DataTableResult(int draw, int recordsTotal, int recordsFilterd, string error, List<T> data)
        {
            Draw = draw;
            RecordsTotal = recordsTotal;
            RecordsFiltered = recordsFilterd;
            Error = error;
            Data = data;
        }
    }
}
