using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Dashboard
{
    public class RegistrationsViewModel
    {
        public string Date { get; set; }
        public int Count { get; set; }

        public RegistrationsViewModel(string date, int count)
        {
            Date = date;
            Count = count;
        }
    }
}
