using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Permission;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Models.Result;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services
{
    internal class PermissionDataService : IPermissionDataService
    {
        private readonly IBaseRepository<PermissionEntity> _permissionRepository;
        private readonly IBaseRepository<PermissionRoleEntity> _permissionRoleRepository;

        private readonly IValidator<DataTableRequest> _dataTableValidator;

        private readonly ILogger<PermissionDataService> _logger;

        public PermissionDataService(IBaseRepository<PermissionEntity> permissionRepository, IBaseRepository<PermissionRoleEntity> permissionRoleRepository,
            IValidator<DataTableRequest> dataTableValidator, ILogger<PermissionDataService> logger)
        {
            _permissionRepository = permissionRepository;
            _permissionRoleRepository = permissionRoleRepository;

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

        public Result<PermissionMenuViewModel> GetMenuViewModel(string id)
        {
            SelectSpecification<PermissionEntity, PermissionMenuViewModel> selectSpecification = new SelectSpecification<PermissionEntity, PermissionMenuViewModel>();
            selectSpecification.AddFilter(X => X.Id == id);
            selectSpecification.AddSelect(x => new PermissionMenuViewModel(
                x.Id,
                x.Name));

            PermissionMenuViewModel permission = _permissionRepository.SingleOrDefault(selectSpecification);
            if (permission == null)
            {
                _logger.LogError($"No Permission");
                return Result.Fail<PermissionMenuViewModel>("no_permission", "No Permission");
            }

            return Result.Ok(permission);
        }

        public Result<DataTableResult<RoleListViewModel>> GetRoles(string id, DataTableRequest dataTableRequest)
        {
            ValidationResult validationResult = _dataTableValidator.Validate(dataTableRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(DataTableRequest)} model");
                return Result.Fail<DataTableResult<RoleListViewModel>>(validationResult.Errors);
            }

            PaginationSpecification<PermissionRoleEntity, RoleListViewModel> paginationSpecification =
                new PaginationSpecification<PermissionRoleEntity, RoleListViewModel>();

            paginationSpecification.AddFilter(x => x.PermissionId == id);

            if(!string.IsNullOrEmpty(dataTableRequest.Search))
            {
                paginationSpecification.AddFilter(x => x.Role.NormalizedName.Contains(dataTableRequest.Search.ToUpper()));
            }

            paginationSpecification.AddSelect(x => new RoleListViewModel(
                x.RoleId,
                x.Role.Name,
                x.Role.Type.ToString()));
            paginationSpecification.AppalyPaging(dataTableRequest.Start, dataTableRequest.Length);

            PaginatedData<RoleListViewModel> paginatedData = _permissionRoleRepository.GetPaginated(paginationSpecification);

            DataTableResult<RoleListViewModel> dataTableResult = new DataTableResult<RoleListViewModel>(
                draw: dataTableRequest.Draw,
                recordsTotal: paginatedData.Count,
                recordsFilterd: paginatedData.Count,
                error: null,
                data: paginatedData.Data);

            return Result.Ok(dataTableResult);
        }

        public Result<PermissionViewModel> GetViewModel(string id)
        {
            SelectSpecification<PermissionEntity, PermissionViewModel> selectSpecification = new SelectSpecification<PermissionEntity, PermissionViewModel>();
            selectSpecification.AddFilter(X => X.Id == id);
            selectSpecification.AddSelect(x => new PermissionViewModel(
                x.Id,
                x.Name,
                x.Description));

            PermissionViewModel permission = _permissionRepository.SingleOrDefault(selectSpecification);
            if(permission == null)
            {
                _logger.LogError($"No Permission");
                return Result.Fail<PermissionViewModel>("no_permission", "No Permission");
            }

            return Result.Ok(permission);
        }
    }
}
