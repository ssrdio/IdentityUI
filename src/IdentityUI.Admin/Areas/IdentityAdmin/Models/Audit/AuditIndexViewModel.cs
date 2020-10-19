using SSRD.AdminUI.Template.Models.Select2;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Audit
{
    public class AuditIndexViewModel
    {
        public List<Select2ItemBase<long?>> ActionTypes { get; set; }
        public List<Select2ItemBase<long?>> SubjectTypes { get; set; }

        public AuditIndexViewModel(List<Select2ItemBase<long?>> actionTypes, List<Select2ItemBase<long?>> subjectTypes)
        {
            ActionTypes = actionTypes;
            SubjectTypes = subjectTypes;
        }
    }
}
