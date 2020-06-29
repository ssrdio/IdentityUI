using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Role;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.Role
{
    internal class RoleAssignmentDataService : IRoleAssignmentDataService
    {
        private readonly IBaseRepository<RoleAssignmentEntity> _roleAssignmentRepository;
        private readonly IBaseRepository<RoleEntity> _roleRepository;
        
        private readonly IValidator<DataTableRequest> _dataTableValidator;
        private readonly IValidator<Select2Request> _select2Validator;

        private readonly ILogger<RoleAssignmentDataService> _logger;

        public RoleAssignmentDataService(IBaseRepository<RoleAssignmentEntity> roleAssignmentRepository,
            IBaseRepository<RoleEntity> roleRepository, IValidator<DataTableRequest> dataTableValidator,
            IValidator<Select2Request> select2Validator, ILogger<RoleAssignmentDataService> logger)
        {
            _roleAssignmentRepository = roleAssignmentRepository;
            _roleRepository = roleRepository;

            _dataTableValidator = dataTableValidator;
            _select2Validator = select2Validator;

            _logger = logger;
        }

        public Result<DataTableResult<RoleAssignmentTableModel>> Get(string roleId, DataTableRequest dataTableRequest)
        {
            ValidationResult validationResult = _dataTableValidator.Validate(dataTableRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(DataTableRequest)} model");
                return Result.Fail<DataTableResult<RoleAssignmentTableModel>>(validationResult.Errors);
            }

            PaginationSpecification<RoleEntity, RoleAssignmentTableModel> paginationSpecification =
                new PaginationSpecification<RoleEntity, RoleAssignmentTableModel>();
            paginationSpecification.AddFilter(x => x.Id != roleId);
            paginationSpecification.AddFilter(x => x.Type == Core.Data.Enums.Entity.RoleTypes.Group);
            if(!string.IsNullOrEmpty(dataTableRequest.Search))
            {
                paginationSpecification.AddFilter(x => x.NormalizedName.Contains(dataTableRequest.Search.ToUpper()));
            }

            paginationSpecification.AddSelect(x => new RoleAssignmentTableModel(
                x.Id,
                x.Name,
                x.CanBeAssignedBy.Any(c => c.RoleId == roleId)));

            paginationSpecification.AppalyPaging(dataTableRequest.Start, dataTableRequest.Length);

            PaginatedData<RoleAssignmentTableModel> paginatedResult = _roleRepository.GetPaginated(paginationSpecification);

            DataTableResult<RoleAssignmentTableModel> dataTableResult = new DataTableResult<RoleAssignmentTableModel>(
                draw: dataTableRequest.Draw,
                recordsTotal: paginatedResult.Count,
                recordsFilterd: paginatedResult.Count,
                error: null,
                data: paginatedResult.Data);

            return Result.Ok(dataTableResult);
        }

        public Result<Select2Result<Select2ItemBase>> GetUnassigned(string roleId, Select2Request select2Request)
        {
            ValidationResult validationResult = _select2Validator.Validate(select2Request);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid Select2Request model");
                return Result.Fail<Select2Result<Select2ItemBase>>(validationResult.Errors);
            }
            
            SelectSpecification<RoleEntity, Select2ItemBase> selectSpecification =
                new SelectSpecification<RoleEntity, Select2ItemBase>();
            selectSpecification.AddFilter(x => x.Id != roleId);
            selectSpecification.AddFilter(x => x.Type == Core.Data.Enums.Entity.RoleTypes.Group);
            selectSpecification.AddFilter(x => !x.CanBeAssignedBy.Select(c => c.RoleId).Contains(roleId));
            selectSpecification.AddSelect(x => new Select2ItemBase(
                x.Id,
                x.Name));

            List<Select2ItemBase> unassignedRoles = _roleRepository.GetList(selectSpecification);

            Select2Result<Select2ItemBase> select2Result = new Select2Result<Select2ItemBase>(
                unassignedRoles, false);

            return Result.Ok(select2Result);
        }
    }
}
