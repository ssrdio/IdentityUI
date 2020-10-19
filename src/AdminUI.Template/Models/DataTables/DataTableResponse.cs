using System.Collections.Generic;

namespace SSRD.AdminUI.Template.Models.DataTables
{
    public class DataTableResult<T>
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public string Error { get; set; }

        public List<T> Data { get; set; }

        public DataTableResult(int draw, int recordsTotal, int recordsFiltered, List<T> data)
        {
            Draw = draw;
            RecordsTotal = recordsTotal;
            RecordsFiltered = recordsFiltered;
            Data = data;
        }

        public DataTableResult(int draw, int recordsTotal, int recordsFiltered, string error, List<T> data)
        {
            Draw = draw;
            RecordsTotal = recordsTotal;
            RecordsFiltered = recordsFiltered;
            Error = error;
            Data = data;
        }

        public DataTableResult(string error)
        {
            Error = error;
        }
    }
}
