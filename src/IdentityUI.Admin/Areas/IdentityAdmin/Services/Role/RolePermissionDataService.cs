using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Role;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.Group
{
    internal class RolePermissionDataService : IRolePermissionsDataService
    {
        private readonly IBaseRepository<PermissionRoleEntity> _permissionRoleRepository;
        private readonly IBaseRepository<PermissionEntity> _permissionRepository;

        private readonly IValidator<DataTableRequest> _dataTableValidator;
        private readonly IValidator<Select2Request> _select2Validator;

        private readonly ILogger<RolePermissionDataService> _logger;

        public RolePermissionDataService(IBaseRepository<PermissionRoleEntity> permissionRoleRepository,
            IBaseRepository<PermissionEntity> PermissionRepository, IValidator<DataTableRequest> dataTableValidator, IValidator<Select2Request> select2Validator,
            ILogger<RolePermissionDataService> logger)
        {
            _permissionRoleRepository = permissionRoleRepository;
            _permissionRepository = PermissionRepository;

            _dataTableValidator = dataTableValidator;
            _select2Validator = select2Validator;

            _logger = logger;
        }

        public Result<DataTableResult<RolePermissionTableModel>> Get(string roleId, DataTableRequest dataTableRequest)
        {
            ValidationResult validationResult = _dataTableValidator.Validate(dataTableRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(DataTableRequest)} model");
                return Result.Fail<DataTableResult<RolePermissionTableModel>>(validationResult.Errors);
            }

            PaginationSpecification<PermissionEntity, RolePermissionTableModel> paginationSpecification =
                new PaginationSpecification<PermissionEntity, RolePermissionTableModel>();

            if(!string.IsNullOrEmpty(dataTableRequest.Search))
            {
                paginationSpecification.AddFilter(x => x.Name.ToUpper().Contains(dataTableRequest.Search.ToUpper()));
            }

            paginationSpecification.AddSelect(x => new RolePermissionTableModel(
                x.Id,
                x.Name,
                x.Roles.Any(c => c.RoleId == roleId)));
            paginationSpecification.AppalyPaging(dataTableRequest.Start, dataTableRequest.Length);

            PaginatedData<RolePermissionTableModel> paginatedData = _permissionRepository.GetPaginated(paginationSpecification);

            DataTableResult<RolePermissionTableModel> dataTableResult = new DataTableResult<RolePermissionTableModel>(
                draw: dataTableRequest.Draw,
                recordsTotal: paginatedData.Count,
                recordsFilterd: paginatedData.Count,
                error: null,
                data: paginatedData.Data);

            return Result.Ok(dataTableResult);
        }

        public Result<Select2Result<Select2ItemBase>> GetAvailable(string roleId, Select2Request select2Request)
        {
            ValidationResult validationResult = _select2Validator.Validate(select2Request);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(Select2Request)} model");
                return Result.Fail<Select2Result<Select2ItemBase>>(validationResult.Errors);
            }

            SelectSpecification<PermissionEntity, Select2ItemBase> selectSpecification = new SelectSpecification<PermissionEntity, Select2ItemBase>();
            selectSpecification.AddFilter(x => !x.Roles.Select(c => c.RoleId).Contains(roleId));
            if(!string.IsNullOrEmpty(select2Request.Term))
            {
                selectSpecification.AddFilter(x => x.Name.ToUpper().Contains(select2Request.Term.ToUpper()));
            }

            selectSpecification.AddSelect(x => new Select2ItemBase(
                x.Id,
                x.Name));

            List<Select2ItemBase> availableRoles = _permissionRepository.GetList(selectSpecification);

            Select2Result<Select2ItemBase> select2Result = new Select2Result<Select2ItemBase>(
                results: availableRoles);

            return Result.Ok(select2Result);
        }
    }
}
