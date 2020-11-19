using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.Audit.Data;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Audit;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services
{
    internal class AuditDataService : IAuditDataService
    {
        private readonly IBaseDAO<AuditEntity> _auditDAO;

        private readonly IValidator<DataTableRequest> _dataTableRequestValidator;
        private readonly IValidator<AuditTableRequest> _auditTableRequestValidator;
        private readonly IValidator<Select2Request> _select2RequestValidator;

        private readonly ILogger<AuditDataService> _logger;

        public AuditDataService(
            IBaseDAO<AuditEntity> auditDAO,
            IValidator<DataTableRequest> dataTableRequestValidator,
            IValidator<AuditTableRequest> auditTableRequestValidator,
            IValidator<Select2Request> select2RequestValidator,
            ILogger<AuditDataService> logger)
        {
            _auditDAO = auditDAO;
            _dataTableRequestValidator = dataTableRequestValidator;
            _auditTableRequestValidator = auditTableRequestValidator;
            _select2RequestValidator = select2RequestValidator;
            _logger = logger;
        }

        public async Task<Result<DataTableResult<AuditAdminTableModel>>> Get(DataTableRequest dataTableRequest, AuditTableRequest auditTableRequest)
        {
            ValidationResult dataTableValidationResult = _dataTableRequestValidator.Validate(dataTableRequest);
            if(!dataTableValidationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(DataTableRequest)} model");
                return Result.Fail<DataTableResult<AuditAdminTableModel>>(dataTableValidationResult.ToResultError());
            }

            if(auditTableRequest == null)
            {
                _logger.LogWarning($"AuditTable request can not be null");
                return Result.Fail<DataTableResult<AuditAdminTableModel>>("can_not_be_null", "Can not be null");
            }

            ValidationResult auditTableRequestValidationResult = _auditTableRequestValidator.Validate(auditTableRequest);
            if (!auditTableRequestValidationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(AuditTableRequest)} model");
                return Result.Fail<DataTableResult<AuditAdminTableModel>>(auditTableRequestValidationResult.ToResultError());
            }

            IBaseSpecificationBuilder<AuditEntity> baseSpecification = SpecificationBuilder
                .Create<AuditEntity>()
                .WithActionType(auditTableRequest.ActionType)
                .WithObjectType(auditTableRequest.ObjectType)
                .WithObjectIdentifier(auditTableRequest.ObjectIdentifier)
                .WithSubjectType(auditTableRequest.SubjectType)
                .WithSubjectIdentifier(auditTableRequest.SubjectIdentifier)
                .WithResourceName(auditTableRequest.ResourceName)
                .InRange(auditTableRequest.From, auditTableRequest.To);

            IBaseSpecification<AuditEntity, AuditAdminTableModel> selectSpecification = baseSpecification
                .OrderBy(x => x.Created, auditTableRequest.OrderBy.Value)
                .Paginate(dataTableRequest.Start, dataTableRequest.Length)
                .Select(x => new AuditAdminTableModel(
                    x.Id,
                    x.ActionType.GetDescription(),
                    x.ObjectType,
                    x.ResourceName,
                    x.SubjectType.GetDescription(),
                    x.SubjectIdentifier,
                    x.Created.ToString("o")))
                .Build();

            int auditCount = await _auditDAO.Count(baseSpecification.Build());
            List<AuditAdminTableModel> audits = await _auditDAO.Get(selectSpecification);

            DataTableResult<AuditAdminTableModel> dataTableResult = new DataTableResult<AuditAdminTableModel>(
                draw: dataTableRequest.Draw,
                recordsTotal: auditCount,
                recordsFiltered: auditCount,
                error: null,
                data: audits);

            return Result.Ok(dataTableResult);
        }

        public async Task<Result<AuditAdminDetailsModel>> Get(long id)
        {

            IBaseSpecification<AuditEntity, AuditAdminDetailsModel> selectSpecification = SpecificationBuilder
                .Create<AuditEntity>()
                .Where(x => x.Id == id)
                .Select(x => new AuditAdminDetailsModel(
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

            AuditAdminDetailsModel auditDetailsModel = await _auditDAO.SingleOrDefault(selectSpecification);
            if (auditDetailsModel == null)
            {
                _logger.LogError($"No audit. AuditId {id}.");
                return Result.Fail<AuditAdminDetailsModel>("no_audit", "No Audit");
            }

            return Result.Ok(auditDetailsModel);
        }

        public Task<Result<Select2Result<Select2Item>>> GetObjectTypes(Select2Request select2Request)
        {
            ValidationResult validationResult = _select2RequestValidator.Validate(select2Request);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {typeof(Select2Request).Name} model");
                return Task.FromResult(Result.Fail<Select2Result<Select2Item>>(validationResult.ToResultError()));
            }

            var specificationBuilder = SpecificationBuilder
                .Create<AuditEntity>()
                .SearchByObjectType(select2Request.Term)
                .Select(x => x.ObjectType)
                .Distinct()
                .OrderByAssending(x => x);

            return GetPaginatedSelect2Result(specificationBuilder, select2Request);
        }

        public Task<Result<Select2Result<Select2Item>>> GetObjectIdentifiers(Select2Request select2Request, string objectType)
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
                .Select(x => x.ObjectIdentifier)
                .Distinct()
                .OrderByAssending(x => x);

            return GetPaginatedSelect2Result(specificationBuilder, select2Request);
        }

        public Task<Result<Select2Result<Select2Item>>> GetSubjectIdentifiers(Select2Request select2Request, SubjectTypes? subjectType)
        {
            ValidationResult validationResult = _select2RequestValidator.Validate(select2Request);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {typeof(Select2Request).Name} model");
                return Task.FromResult(Result.Fail<Select2Result<Select2Item>>(validationResult.ToResultError()));
            }

            var specificationBuilder = SpecificationBuilder
                .Create<AuditEntity>()
                .WithSubjectType(subjectType)
                .SearchBySubjectIdentifier(select2Request.Term)
                .Select(x => x.SubjectIdentifier)
                .Distinct()
                .OrderByAssending(x => x);

            return GetPaginatedSelect2Result(specificationBuilder, select2Request);
        }

        public Task<Result<Select2Result<Select2Item>>> GetResourceNames(Select2Request select2Request)
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
                .Select(x => x.ResourceName)
                .Distinct()
                .OrderByAssending(x => x);

            return GetPaginatedSelect2Result(specificationBuilder, select2Request);
        }

        public AuditIndexViewModel GetIndexViewModel()
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
                actionTypes: actionTypes,
                subjectTypes: subjectTypes);

            return auditIndexViewModel;
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
    }
}
