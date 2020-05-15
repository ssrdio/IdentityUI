using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Setting.Email
{
    public class EmailTableModel
    {
        public long Id { get; set; }
        public string Type { get; set; }

        public EmailTableModel(long id, string type)
        {
            Id = id;
            Type = type;
        }
    }
}
