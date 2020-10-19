using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Audit
{
    public class AuditDetailsModel
    {
        public long Id { get; set; }
        public string ActionType { get; set; }
        public string Created { get; set; }
        public string ResourceName { get; set; }
        public string Metadata { get; set; }

        public AuditDetailsModel(long id, string actionType, string created, string resourceName, string metadata)
        {
            Id = id;
            ActionType = actionType;
            Created = created;
            ResourceName = resourceName;
            Metadata = metadata;
        }
    }
}
