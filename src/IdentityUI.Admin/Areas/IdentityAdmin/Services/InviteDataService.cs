using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Invite;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services
{
    public class InviteDataService : IInviteDataService
    {
        private readonly IBaseRepository<InviteEntity> _inviteRepository;

        private readonly IValidator<DataTableRequest> _dataTableValidator;

        private readonly ILogger<InviteDataService> _logger;

        public InviteDataService(IBaseRepository<InviteEntity> inviteRepository, IValidator<DataTableRequest> dataTableValidator, ILogger<InviteDataService> logger)
        {
            _inviteRepository = inviteRepository;
            _dataTableValidator = dataTableValidator;
            _logger = logger;
        }

        public Result<DataTableResult<InviteTableModel>> Get(DataTableRequest dataTableRequest)
        {
            ValidationResult validationResult = _dataTableValidator.Validate(dataTableRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(DataTableRequest)} model");
                return Result.Fail<DataTableResult<InviteTableModel>>(validationResult.Errors);
            }

            PaginationSpecification<InviteEntity, InviteTableModel> paginationSpecification = new PaginationSpecification<InviteEntity, InviteTableModel>();
            if(!string.IsNullOrEmpty(dataTableRequest.Search))
            {
                paginationSpecification.AddFilter(x => x.Email.ToUpper().Contains(dataTableRequest.Search.ToUpper()));
            }

            paginationSpecification.AddSelect(x => new InviteTableModel(
                x.Id,
                x.Email,
                x.Status.ToString(),
                x.ExpiresAt.ToString(DateTimeFormats.DEFAULT_DATE_TIME_FORMAT)));
            paginationSpecification.AppalyPaging(dataTableRequest.Start, dataTableRequest.Length);

            PaginatedData<InviteTableModel> paginatedData = _inviteRepository.GetPaginated(paginationSpecification);

            DataTableResult<InviteTableModel> dataTableResult = new DataTableResult<InviteTableModel>(
                draw: dataTableRequest.Draw,
                recordsTotal: paginatedData.Count,
                recordsFilterd: paginatedData.Count,
                error: null,
                data: paginatedData.Data);

            return Result.Ok(dataTableResult);
        }
    }
}
