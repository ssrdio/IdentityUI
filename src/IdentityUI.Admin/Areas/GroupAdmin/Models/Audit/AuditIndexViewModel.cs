using SSRD.AdminUI.Template.Models.Select2;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Audit
{
    public class AuditIndexViewModel : GroupAdminViewModel
    {
        public List<Select2ItemBase<long?>> ActionTypes { get; set; }
        public List<Select2ItemBase<long?>> SubjectTypes { get; set; }

        public AuditIndexViewModel(
            string groupId,
            List<Select2ItemBase<long?>> actionTypes,
            List<Select2ItemBase<long?>> subjectTypes) : base(groupId)
        {
            ActionTypes = actionTypes;
            SubjectTypes = subjectTypes;
        }
    }
}
