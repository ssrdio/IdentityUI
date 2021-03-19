using Microsoft.Extensions.Logging;
using SSRD.Audit.Data;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Audit
{
    public class AuditService : IAuditService
    {
        private const string AUDIT_NOT_FOUND = "audit_not_found";
        private const string FAILED_TO_ADD_AUDIT_COMMENT = "failed_to_add_audit_comment";

        private readonly IBaseDAO<AuditEntity> _auditDAO;
        private readonly IBaseDAO<AuditCommentEntity> _auditCommentDAO;

        private readonly IIdentityUIUserInfoService _identityUIUserInfoService;

        private readonly ILogger<AuditService> _logger;

        public AuditService(
            IBaseDAO<AuditEntity> auditDAO,
            IBaseDAO<AuditCommentEntity> auditCommentDAO,
            IIdentityUIUserInfoService identityUIUserInfoService,
            ILogger<AuditService> logger)
        {
            _auditDAO = auditDAO;
            _auditCommentDAO = auditCommentDAO;
            _identityUIUserInfoService = identityUIUserInfoService;
            _logger = logger;
        }

        private async Task<Result> Exists(long id)
        {
            IBaseSpecification<AuditEntity, AuditEntity> specification = SpecificationBuilder
                .Create<AuditEntity>()
                .Where(x => x.Id == id)
                .Build();

            bool auditExists = await _auditDAO.Exist(specification);
            if(!auditExists)
            {
                _logger.LogError($"Audit not found. Id {id}");
                return Result.Fail(AUDIT_NOT_FOUND);
            }

            return Result.Ok();
        }

        public async Task<Result> AddComment(long id, AddAuditCommentModel addAuditComment)
        {
            Result auditExists = await Exists(id);
            if(auditExists.Failure)
            {
                return Result.Fail(auditExists);
            }

            AuditCommentEntity auditCommentEntity = new AuditCommentEntity(
                comment: addAuditComment.Comment,
                auditId: id,
                userId: _identityUIUserInfoService.GetUserId(),
                groupId: _identityUIUserInfoService.GetGroupId());

            bool addResult = await _auditCommentDAO.Add(auditCommentEntity);
            if(!addResult)
            {
                _logger.LogError($"Failed to add audit comment. AuditId {id}");
                return Result.Fail(FAILED_TO_ADD_AUDIT_COMMENT);
            }

            return Result.Ok();
        }
    }
}
