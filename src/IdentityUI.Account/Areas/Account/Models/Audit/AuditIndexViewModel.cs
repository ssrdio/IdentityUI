using SSRD.AdminUI.Template.Models.Select2;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Audit
{
    public class AuditIndexViewModel
    {
        public List<Select2ItemBase<long?>> ActionTypes { get; set; }

        public AuditIndexViewModel(List<Select2ItemBase<long?>> actionTypes)
        {
            ActionTypes = actionTypes;
        }
    }
}
