using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.Audit.Data;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Audit;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Services
{
    public class GroupAdminAuditDataService : IGroupAdminAuditDataService
    {
        private readonly IBaseDAO<AuditEntity> _auditDAO;

        private readonly IValidator<DataTableRequest> _dataTableRequestValidator;
        private readonly IValidator<GroupAdminAuditTableRequest> _auditTableRequestValidator;
        private readonly IValidator<Select2Request> _select2RequestValidator;

        private readonly ILogger<GroupAdminAuditDataService> _logger;

        public GroupAdminAuditDataService(
            IBaseDAO<AuditEntity> auditDAO,
            IValidator<DataTableRequest> dataTableRequestValidator,
            IValidator<GroupAdminAuditTableRequest> auditTableRequestValidator,
            IValidator<Select2Request> select2RequestValidator,
            ILogger<GroupAdminAuditDataService> logger)
        {
            _auditDAO = auditDAO;
            _dataTableRequestValidator = dataTableRequestValidator;
            _auditTableRequestValidator = auditTableRequestValidator;
            _select2RequestValidator = select2RequestValidator;
            _logger = logger;
        }

        public Task<Result<AuditIndexViewModel>> GetIndexViewModel(string groupId)
        {
            List<Select2ItemBase<long?>> actionTypes = Enum.GetValues(typeof(ActionTypes))
                .Cast<ActionTypes>()
                .Select(x => new Select2ItemBase<long?>(
                    id: (long)x,
                    text: x.GetDescription()))
                .ToList();

            List<Select2ItemBase<long?>> subjectTypes = Enum.GetValues(typeof(SubjectTypes))
                .Cast<SubjectTypes>()
                .Select(x => new Select2ItemBase<long?>(
                    id: (long)x,
                    text: x.GetDescription()))
                .ToList();

            AuditIndexViewModel auditIndexViewModel = new AuditIndexViewModel(
                groupId: groupId,
                actionTypes: actionTypes,
                subjectTypes: subjectTypes);

            return Task.FromResult(Result.Ok(auditIndexViewModel));
        }

        public Task<Result<Select2Result<Select2Item>>> GetObjectTypes(string groupId, Select2Request select2Request)
        {
            ValidationResult validationResult = _select2RequestValidator.Validate(select2Request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {typeof(Select2Request).Name} model");
                return Task.FromResult(Result.Fail<Select2Result<Select2Item>>(validationResult.ToResultError()));
            }

            var specificationBuilder = SpecificationBuilder
                .Create<AuditEntity>()
                .SearchByObjectType(select2Request.Term)
                .WithGroupIdentifier(groupId)
                .Select(x => x.ObjectType)
                .Distinct()
                .OrderByAssending(x => x);

            return GetPaginatedSelect2Result(specificationBuilder, select2Request);
        }

        public Task<Result<Select2Result<Select2Item>>> GetObjectIdentifiers(string groupId, Select2Request select2Request, string objectType)
        {
            ValidationResult validationResult = _select2RequestValidator.Validate(select2Request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {typeof(Select2Request).Name} model");
                return Task.FromResult(Result.Fail<Select2Result<Select2Item>>(validationResult.ToResultError()));
            }


            if (string.IsNullOrEmpty(objectType))
            {
                return Task.FromResult(Result.Fail<Select2Result<Select2Item>>("invalid_model", "Invalid Model"));
            }

            var specificationBuilder = SpecificationBuilder
                .Create<AuditEntity>()
                .WithObjectType(objectType)
                .SearchByObjectIdentifier(select2Request.Term)
                .WithGroupIdentifier(groupId)
                .Select(x => x.ObjectIdentifier)
                .Distinct()
                .OrderByAssending(x => x);

            return GetPaginatedSelect2Result(specificationBuilder, select2Request);
        }

        public Task<Result<Select2Result<Select2Item>>> GetSubjectIdentifiers(string groupId, Select2Request select2Request, SubjectTypes? subjectType)
        {
            ValidationResult validationResult = _select2RequestValidator.Validate(select2Request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {typeof(Select2Request).Name} model");
                return Task.FromResult(Result.Fail<Select2Result<Select2Item>>(validationResult.ToResultError()));
            }

            var specificationBuilder = SpecificationBuilder
                .Create<AuditEntity>()
                .WithSubjectType(subjectType)
                .SearchBySubjectIdentifier(select2Request.Term)
                .WithGroupIdentifier(groupId)
                .Select(x => x.SubjectIdentifier)
                .Distinct()
                .OrderByAssending(x => x);

            return GetPaginatedSelect2Result(specificationBuilder, select2Request);
        }

        public Task<Result<Select2Result<Select2Item>>> GetResourceNames(string groupId, Select2Request select2Request)
        {
            ValidationResult validationResult = _select2RequestValidator.Validate(select2Request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {typeof(Select2Request).Name} model");
                return Task.FromResult(Result.Fail<Select2Result<Select2Item>>(validationResult.ToResultError()));
            }

            var specificationBuilder = SpecificationBuilder
                .Create<AuditEntity>()
                .SearchByResourceName(select2Request.Term)
                .WithGroupIdentifier(groupId)
                .Select(x => x.ResourceName)
                .Distinct()
                .OrderByAssending(x => x);

            return GetPaginatedSelect2Result(specificationBuilder, select2Request);
        }

        private async Task<Result<Select2Result<Select2Item>>> GetPaginatedSelect2Result<TResult>(
            ISelectSpecificationBuilder<AuditEntity, TResult> specificationBuilder,
            Select2Request select2Request)
        {
            int count = await _auditDAO.Count(specificationBuilder.Build());

            var selectSpecification = specificationBuilder
                .Paginate(select2Request.GetPageStart(), select2Request.GetPageLenght())
                .Build();

            List<TResult> objectTypes = await _auditDAO.Get(selectSpecification);

            Select2Result<Select2Item> select2Result = new Select2Result<Select2Item>(
                objectTypes
                    .Select(x => new Select2Item(
                        id: x?.ToString(),
                        text: x?.ToString()))
                    .ToList(),
                pagination: select2Request.IsMore(count));

            return Result.Ok(select2Result);
        }

        public async Task<Result<DataTableResult<GroupAdminAuditTableModel>>> Get(
            string groupId,
            DataTableRequest dataTableRequest,
            GroupAdminAuditTableRequest auditTableRequest)
        {
            ValidationResult dataTableValidationResult = _dataTableRequestValidator.Validate(dataTableRequest);
            if (!dataTableValidationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(DataTableRequest)} model");
                return Result.Fail<DataTableResult<GroupAdminAuditTableModel>>(dataTableValidationResult.ToResultError());
            }

            if (auditTableRequest == null)
            {
                _logger.LogWarning($"AuditTable request can not be null");
                return Result.Fail<DataTableResult<GroupAdminAuditTableModel>>("can_not_be_null", "Can not be null");
            }

            ValidationResult auditTableRequestValidationResult = _auditTableRequestValidator.Validate(auditTableRequest);
            if (!auditTableRequestValidationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(GroupAdminAuditTableRequest)} model");
                return Result.Fail<DataTableResult<GroupAdminAuditTableModel>>(auditTableRequestValidationResult.ToResultError());
            }

            IBaseSpecificationBuilder<AuditEntity> baseSpecification = SpecificationBuilder
                .Create<AuditEntity>()
                .WithActionType(auditTableRequest.ActionType)
                .WithObjectType(auditTableRequest.ObjectType)
                .WithObjectIdentifier(auditTableRequest.ObjectIdentifier)
                .WithSubjectType(auditTableRequest.SubjectType)
                .WithSubjectIdentifier(auditTableRequest.SubjectIdentifier)
                .WithResourceName(auditTableRequest.ResourceName)
                .InRange(auditTableRequest.From, auditTableRequest.To)
                .WithGroupIdentifier(groupId);

            IBaseSpecification<AuditEntity, GroupAdminAuditTableModel> selectSpecification = baseSpecification
                .OrderBy(x => x.Created, auditTableRequest.OrderBy.Value)
                .Paginate(dataTableRequest.Start, dataTableRequest.Length)
                .Select(x => new GroupAdminAuditTableModel(
                    x.Id,
                    x.ActionType.GetDescription(),
                    x.ObjectType,
                    x.ResourceName,
                    x.SubjectType.GetDescription(),
                    x.SubjectIdentifier,
                    x.Created.ToString("o")))
                .Build();

            int auditCount = await _auditDAO.Count(baseSpecification.Build());
            List<GroupAdminAuditTableModel> audits = await _auditDAO.Get(selectSpecification);

            DataTableResult<GroupAdminAuditTableModel> dataTableResult = new DataTableResult<GroupAdminAuditTableModel>(
                draw: dataTableRequest.Draw,
                recordsTotal: auditCount,
                recordsFiltered: auditCount,
                data: audits);

            return Result.Ok(dataTableResult);
        }

        public async Task<Result<GroupAdminAuditDetailsModel>> Get(string groupId, long id)
        {
            IBaseSpecification<AuditEntity, GroupAdminAuditDetailsModel> selectSpecification = SpecificationBuilder
                .Create<AuditEntity>()
                .Where(x => x.Id == id)
                .WithGroupIdentifier(groupId)
                .Select(x => new GroupAdminAuditDetailsModel(
                    x.Id,
                    x.ActionType.GetDescription(),
                    x.ObjectType,
                    x.ObjectIdentifier,
                    x.ObjectMetadata,
                    x.SubjectType.GetDescription(),
                    x.SubjectIdentifier,
                    x.SubjectMetadata,
                    x.GroupIdentifier,
                    x.Host,
                    x.RemoteIp,
                    x.ResourceName,
                    x.UserAgent,
                    x.TraceIdentifier,
                    x.AppVersion,
                    x.Metadata,
                    x.Created.ToString("o")))
                .Build();

            GroupAdminAuditDetailsModel auditDetailsModel = await _auditDAO.SingleOrDefault(selectSpecification);
            if (auditDetailsModel == null)
            {
                _logger.LogError($"No audit. AuditId {id}.");
                return Result.Fail<GroupAdminAuditDetailsModel>("no_audit", "No Audit");
            }

            return Result.Ok(auditDetailsModel);
        }
    }
}
