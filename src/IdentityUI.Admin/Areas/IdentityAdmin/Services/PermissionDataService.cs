using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Permission;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services
{
    internal class PermissionDataService : IPermissionDataService
    {
        private readonly IBaseRepository<PermissionEntity> _permissionRepository;

        private readonly IValidator<DataTableRequest> _dataTableValidator;

        private readonly ILogger<PermissionDataService> _logger;

        public PermissionDataService(IBaseRepository<PermissionEntity> permissionRepository, IValidator<DataTableRequest> dataTableValidator,
            ILogger<PermissionDataService> logger)
        {
            _permissionRepository = permissionRepository;
            _dataTableValidator = dataTableValidator;
            _logger = logger;
        }

        public Result<DataTableResult<PermissionTableModel>> Get(DataTableRequest dataTableRequest)
        {
            ValidationResult validationResult = _dataTableValidator.Validate(dataTableRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(DataTableRequest)} model");
                return Result.Fail<DataTableResult<PermissionTableModel>>(validationResult.Errors);
            }

            PaginationSpecification<PermissionEntity, PermissionTableModel> paginationSpecification =
                new PaginationSpecification<PermissionEntity, PermissionTableModel>();

            if(!string.IsNullOrEmpty(dataTableRequest.Search))
            {
                paginationSpecification.AddFilter(x => x.Name.ToUpper().Contains(dataTableRequest.Search.ToUpper()));
            }

            paginationSpecification.AddSelect(x => new PermissionTableModel(
                x.Id,
                x.Name));
            paginationSpecification.AppalyPaging(dataTableRequest.Start, dataTableRequest.Length);

            PaginatedData<PermissionTableModel> paginatedData = _permissionRepository.GetPaginated(paginationSpecification);

            DataTableResult<PermissionTableModel> dataTableResult = new DataTableResult<PermissionTableModel>(
                draw: dataTableRequest.Draw,
                recordsTotal: paginatedData.Count,
                recordsFilterd: paginatedData.Count,
                error: null,
                data: paginatedData.Data);

            return Result.Ok(dataTableResult);
        }
    }
}
