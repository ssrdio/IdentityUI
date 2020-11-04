using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.Audit.Data;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using SSRD.IdentityUI.Account.Areas.Account.Models.Audit;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Services
{
    internal class AuditDataService : IAuditDataService
    {
        private readonly IBaseDAO<AuditEntity> _auditDAO;
        private readonly IIdentityUIUserInfoService _identityUIUserInfoService;

        private readonly IValidator<DataTableRequest> _dataTableRequestValidator;
        private readonly IValidator<AuditTableRequest> _auditTableRequestValidator;

        private readonly ILogger<AuditDataService> _logger;

        public AuditDataService(
            IBaseDAO<AuditEntity> auditDAO,
            IIdentityUIUserInfoService identityUIUserInfoService,
            IValidator<DataTableRequest> dataTableRequestValidator,
            IValidator<AuditTableRequest> auditTableRequestValidator,
            ILogger<AuditDataService> logger)
        {
            _auditDAO = auditDAO;
            _identityUIUserInfoService = identityUIUserInfoService;

            _dataTableRequestValidator = dataTableRequestValidator;
            _auditTableRequestValidator = auditTableRequestValidator;
            _logger = logger;
        }

        public async Task<Result<DataTableResult<AuditTableModel>>> Get(DataTableRequest dataTableRequest, AuditTableRequest auditTableRequest)
        {
            ValidationResult validationResult = _dataTableRequestValidator.Validate(dataTableRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(DataTableRequest)} model");
                return Result.Fail<DataTableResult<AuditTableModel>>(validationResult.ToResultError());
            }

            ValidationResult auditTabeValidationResult = _auditTableRequestValidator.Validate(auditTableRequest);
            if(!auditTabeValidationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {typeof(AuditTableRequest).Name} model");
                return Result.Fail<DataTableResult<AuditTableModel>>(validationResult.ToResultError());
            }

            IBaseSpecificationBuilder<AuditEntity> baseSpecification = SpecificationBuilder
                .Create<AuditEntity>()
                .WithActionType(auditTableRequest.ActionType)
                .InRange(auditTableRequest.From?.UtcDateTime, auditTableRequest.To?.UtcDateTime)
                .WithUser(_identityUIUserInfoService.GetUserId());

            IBaseSpecification<AuditEntity, AuditTableModel> selectSpecification = baseSpecification
                .OrderBy(x => x.Created, auditTableRequest.OrderBy.Value)
                .Paginate(dataTableRequest.Start, dataTableRequest.Length)
                .Select(x => new AuditTableModel(
                    x.Id,
                    x.ActionType.GetDescription(),
                    x.ResourceName,
                    x.Created.ToString("o")))
                .Build();

            int auditsCount = await _auditDAO.Count(baseSpecification.Build());
            List<AuditTableModel> audits = await _auditDAO.Get(selectSpecification);

            DataTableResult<AuditTableModel> dataTableResult = new DataTableResult<AuditTableModel>(
                draw: dataTableRequest.Draw,
                recordsTotal: auditsCount,
                recordsFiltered: auditsCount,
                data: audits);

            return Result.Ok(dataTableResult);
        }

        public async Task<Result<AuditDetailsModel>> Get(long id)
        {
            string userId = _identityUIUserInfoService.GetUserId();

            IBaseSpecification<AuditEntity, AuditDetailsModel> selectSpecification = SpecificationBuilder
                .Create<AuditEntity>()
                .Where(x => x.Id == id)
                .WithUser(userId)
                .Select(x => new AuditDetailsModel(
                    x.Id,
                    x.ActionType.GetDescription(),
                    x.Created.ToString("o"),
                    x.ResourceName,
                    x.ObjectMetadata))
                .Build();

            AuditDetailsModel auditDetailsModal = await _auditDAO.SingleOrDefault(selectSpecification);
            if(auditDetailsModal == null)
            {
                _logger.LogError($"No audit. AuditId {id}, UserId {userId}");
                return Result.Fail<AuditDetailsModel>("no_audit", "No Audit");
            }

            return Result.Ok(auditDetailsModal);
        }

        public AuditIndexViewModel GetIndexViewModel()
        {
            List<Select2ItemBase<long?>> actionTypes = Enum.GetValues(typeof(ActionTypes))
                .Cast<ActionTypes>()
                .Select(x => new Select2ItemBase<long?>(
                    id: (long)x,
                    text: x.ToString()))
                .ToList();

            actionTypes.Insert(0, new Select2ItemBase<long?>(
                id: null,
                text: "All"));

            AuditIndexViewModel auditIndexViewModel = new AuditIndexViewModel(
                actionTypes: actionTypes);

            return auditIndexViewModel;
        }
    }
}
