using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.Audit.Models
{
    public class ProccessChangeTrackerResult
    {
        public bool RequiresCustomBatch { get; set; }
        public IEnumerable<AuditObjectData> AuditObjectData { get; set; }

        public ProccessChangeTrackerResult(bool requiresCustomBatch, IEnumerable<AuditObjectData> auditObjectData)
        {
            RequiresCustomBatch = requiresCustomBatch;
            AuditObjectData = auditObjectData;
        }
    }
}
