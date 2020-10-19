using SSRD.Audit.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.Audit.Services
{
    public interface IAuditSubjectDataService
    {
        AuditSubjectData Get();
    }
}
