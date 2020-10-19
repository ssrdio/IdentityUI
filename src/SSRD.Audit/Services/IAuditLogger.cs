using SSRD.Audit.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.Audit.Services
{
    public interface IAuditLogger
    {
        void Log(AuditObjectData auditData);
        void Log(IEnumerable<AuditObjectData> auditData);
        Task LogAsync(AuditObjectData auditData);
        Task LogAsync(IEnumerable<AuditObjectData> auditData);
    }
}
