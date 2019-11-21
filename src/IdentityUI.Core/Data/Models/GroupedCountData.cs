using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Models
{
    public class GroupedCountData
    {
        public DateTimeOffset Date { get; set; }
        public int Count { get; set; }

        public GroupedCountData(DateTimeOffset date, int count)
        {
            Date = date;
            Count = count;
        }
    }
}
