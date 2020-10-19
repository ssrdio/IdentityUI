using Microsoft.Extensions.Logging;
using SSRD.Audit.Data;
using SSRD.Audit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.Audit.Services
{
    public class AuditDbLogger : IAuditLogger
    {
        private readonly IAuditDbContext _dbContext;
        private readonly IAuditSubjectDataService _auditDataService;

        private readonly ILogger<AuditDbLogger> _logger;

        public AuditDbLogger(IAuditDbContext dbContext, IAuditSubjectDataService auditDataService, ILogger<AuditDbLogger> logger)
        {
            _dbContext = dbContext;
            _auditDataService = auditDataService;

            _logger = logger;
        }

        public void Log(AuditObjectData auditObject)
        {
            AuditSubjectData auditSubjectData = _auditDataService.Get();

            AuditEntity audit = new AuditEntity(
                auditObjectData: auditObject,
                auditSubjectData: auditSubjectData);

            _dbContext.Audit.Add(audit);

            int changes = _dbContext.SaveChanges();
            if(changes <= 0)
            {
                _logger.LogError($"Failed to add audit data");
            }
        }

        public void Log(IEnumerable<AuditObjectData> auditObjects)
        {
            if (auditObjects.Count() == 0)
            {
                return;
            }

            AuditSubjectData auditSubjectData = _auditDataService.Get();

            IEnumerable<AuditEntity> auditList = auditObjects
                .Select(x => new AuditEntity(
                    auditObjectData: x,
                    auditSubjectData: auditSubjectData));

            _dbContext.Audit.AddRange(auditList);

            int changes = _dbContext.SaveChanges();
            if (changes <= 0)
            {
                _logger.LogError($"Failed to add audit data");
            }
        }

        public async Task LogAsync(AuditObjectData auditObject)
        {
            AuditSubjectData auditSubjectData = _auditDataService.Get();

            AuditEntity audit = new AuditEntity(
                auditObjectData: auditObject,
                auditSubjectData: auditSubjectData);

            _dbContext.Audit.Add(audit);

            int changes = await _dbContext.SaveChangesAsync();
            if (changes <= 0)
            {
                _logger.LogError($"Failed to add audit");
            }
        }

        public async Task LogAsync(IEnumerable<AuditObjectData> auditObjects)
        {
            if(auditObjects.Count() == 0)
            {
                return;
            }

            AuditSubjectData auditSubjectData = _auditDataService.Get();

            IEnumerable<AuditEntity> auditList = auditObjects
                .Select(x => new AuditEntity(
                    auditObjectData: x,
                    auditSubjectData: auditSubjectData));

            _dbContext.Audit.AddRange(auditList);

            int changes = await _dbContext.SaveChangesAsync();
            if (changes <= 0)
            {
                _logger.LogError($"Failed to add audit");
            }
        }
    }
}
