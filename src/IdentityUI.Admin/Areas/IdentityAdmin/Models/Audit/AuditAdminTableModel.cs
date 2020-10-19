using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Audit
{
    public class AuditAdminTableModel
    {
        public long Id { get; set; }
        public string ActionType { get; set; }
        public string ObjectType { get; set; }
        public string ResourceName { get; set; }
        public string SubjectType { get; set; }
        public string SubjectIdentifier { get; set; }
        public string Created { get; set; }

        public AuditAdminTableModel(long id, string actionType, string objectType, string resourceName, string subjectType, string subjectIdentifier, string created)
        {
            Id = id;
            ActionType = actionType;
            ObjectType = objectType;
            ResourceName = resourceName;
            SubjectType = subjectType;
            SubjectIdentifier = subjectIdentifier;
            Created = created;
        }
    }
}
