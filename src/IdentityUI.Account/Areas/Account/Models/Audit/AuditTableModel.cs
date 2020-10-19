using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Audit
{
    public class AuditTableModel
    {
        public long Id { get; set; }
        public string ActionType { get; set; }
        public string ResourceName { get; set; }
        public string Created { get; set; }

        public AuditTableModel(long id, string actionType, string resourceName, string created)
        {
            Id = id;
            ActionType = actionType;
            ResourceName = resourceName;
            Created = created;
        }
    }
}
