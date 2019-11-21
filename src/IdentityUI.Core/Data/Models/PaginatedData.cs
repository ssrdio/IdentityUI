using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Models
{
    public class PaginatedData<T>
    {
        public List<T> Data { get; set; }
        public int Count { get; set; }

        public PaginatedData(List<T> data, int count)
        {
            Data = data;
            Count = count;
        }
    }
}
