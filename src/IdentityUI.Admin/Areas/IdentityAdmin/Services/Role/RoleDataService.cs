using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using FluentValidation.Results;
using System.Linq;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Interfaces.Data;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role;
using SSRD.AdminUI.Template.Models;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Runtime.CompilerServices;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Role;
using SSRD.IdentityUI.Core.Data.Entities.Group;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.Role
{
    internal class RoleDataService : IRoleDataService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IBaseRepository<GroupUserEntity> _groupUserRepository;

        private readonly IValidator<DataTableRequest> _dataTableValidator;

        private readonly ILogger<RoleDataService> _logger;

        public RoleDataService(IRoleRepository roleRepository, IUserRoleRepository userRoleRepository,
            IBaseRepository<GroupUserEntity> groupUserRepository, IValidator<DataTableRequest> dataTableValidator,
            ILogger<RoleDataService> logger)
        {
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _groupUserRepository = groupUserRepository;

            _dataTableValidator = dataTableValidator;

            _logger = logger;
        }

        private IEnumerable<SelectListItem> GetRoleTypesList(RoleTypes? selected = null)
        {
            RoleTypes[] roleTypes = new RoleTypes[]
            {
                RoleTypes.Global,
                RoleTypes.Group,
            };

            IEnumerable<SelectListItem> selectListItems = roleTypes
                .Select(x => new SelectListItem(
                    text: $"{x}",
                    value: ((int)x).ToString(),
                    selected: selected.HasValue ? x == selected : false));

            return selectListItems;
        }

        public Result<DataTableResult<RoleListViewModel>> GetAll(DataTableRequest request)
        {
            ValidationResult validationResult = _dataTableValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid DataTableRequest model");
                return Result.Fail<DataTableResult<RoleListViewModel>>(ResultUtils.ToResultError(validationResult.Errors.ToList()));
            }

            PaginationSpecification<RoleEntity, RoleListViewModel> baseSpecification = new PaginationSpecification<RoleEntity, RoleListViewModel>();
            if (!string.IsNullOrEmpty(request.Search))
            {
                string search = request.Search.ToUpper();

                baseSpecification.AddFilter(x =>
                    x.Id.ToUpper().Contains(search)
                    || x.Name.ToUpper().Contains(search));
            }

            baseSpecification.AppalyPaging(request.Start, request.Length);
            baseSpecification.AddSelect(x => new RoleListViewModel(
                x.Id,
                x.Name,
                x.Type.ToString()));

            PaginatedData<RoleListViewModel> pagedResult = _roleRepository.GetPaginated(baseSpecification);

            DataTableResult<RoleListViewModel> result = new DataTableResult<RoleListViewModel>(
                draw: request.Draw,
                recordsTotal: pagedResult.Count,
                recordsFilterd: pagedResult.Count,
                error: null,
                data: pagedResult.Data);

            return Result.Ok(result);
        }

        public Result<RoleDetailViewModel> GetDetails(string id)
        {
            SelectSpecification<RoleEntity, RoleDetailViewModel> roleSpecification = new SelectSpecification<RoleEntity, RoleDetailViewModel>();
            roleSpecification.AddFilter(x => x.Id == id);
            roleSpecification.AddSelect(x => new RoleDetailViewModel(
                x.Id,
                x.Name,
                x.Description,
                x.Type));

            RoleDetailViewModel roleDetail = _roleRepository.SingleOrDefault(roleSpecification);
            if (roleDetail == null)
            {
                _logger.LogWarning($"Role with id {id} does not exist");
                return Result.Fail<RoleDetailViewModel>("no_role", "No Role");
            }

            roleDetail.RoleTypes = GetRoleTypesList(roleDetail.Type);

            return Result.Ok(roleDetail);
        }

        public NewRoleViewModel GetNewRoleViewModel(Result result = null)
        {
            StatusAlertViewModel statusAlert = null;
            if(result != null)
            {
                statusAlert = StatusAlertViewExtension.Get(result);
            }

            NewRoleViewModel newRoleViewModel = new NewRoleViewModel(
                statusAlert: statusAlert,
                roleTypes: GetRoleTypesList());

            return newRoleViewModel;
        }

        public Result<DataTableResult<UserTableModel>> GetGlobalUsers(string roleId, DataTableRequest request)
        {
            ValidationResult validationResult = _dataTableValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid DataTableRequest model");
                return Result.Fail<DataTableResult<UserTableModel>>(ResultUtils.ToResultError(validationResult.Errors.ToList()));
            }

            BaseSpecification<RoleEntity> roleSpecification = new BaseSpecification<RoleEntity>();
            roleSpecification.AddFilter(x => x.Id == roleId);
            roleSpecification.AddFilter(x => x.Type == RoleTypes.Global);

            bool existResult = _roleRepository.Exist(roleSpecification);
            if (!existResult)
            {
                _logger.LogWarning($"GlobalRole with id {roleId} does not exist");
                return Result.Fail<DataTableResult<UserTableModel>>("no_role", "No Role");
            }

            PaginationSpecification<UserRoleEntity, UserTableModel> baseSpecification = new PaginationSpecification<UserRoleEntity, UserTableModel>();
            baseSpecification.AddFilter(x => x.RoleId == roleId);
            baseSpecification.AddSelect(x => new UserTableModel(
                x.User.Id,
                x.User.UserName,
                x.User.Email));

            if (!string.IsNullOrEmpty(request.Search))
            {
                string search = request.Search.ToUpper();

                baseSpecification.AddFilter(x =>
                    x.User.Id.ToUpper().Contains(search)
                    || x.User.Email.ToUpper().Contains(search)
                    || x.User.UserName.ToUpper().Contains(search)
                    || x.User.FirstName.ToUpper().Contains(search)
                    || x.User.LastName.ToUpper().Contains(search));
            }
            baseSpecification.AppalyPaging(request.Start, request.Length);
            baseSpecification.AddInclude(x => x.User);

            PaginatedData<UserTableModel> paginationData = _userRoleRepository.GetPaginated(baseSpecification);

            DataTableResult<UserTableModel> result = new DataTableResult<UserTableModel>(
                draw: request.Draw,
                recordsTotal: paginationData.Count,
                recordsFilterd: paginationData.Count,
                error: null,
                data: paginationData.Data);

            return Result.Ok(result);
        }

        public Result<DataTableResult<UserTableModel>> GetGroupUsers(string roleId, DataTableRequest request)
        {
            ValidationResult validationResult = _dataTableValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(DataTableRequest)} model");
                return Result.Fail<DataTableResult<UserTableModel>>(ResultUtils.ToResultError(validationResult.Errors.ToList()));
            }

            BaseSpecification<RoleEntity> roleSpecification = new BaseSpecification<RoleEntity>();
            roleSpecification.AddFilter(x => x.Id == roleId);
            roleSpecification.AddFilter(x => x.Type == RoleTypes.Group);

            bool existResult = _roleRepository.Exist(roleSpecification);
            if (!existResult)
            {
                _logger.LogWarning($"GroupRole with id {roleId} does not exist");
                return Result.Fail<DataTableResult<UserTableModel>>("no_role", "No Role");
            }

            PaginationSpecification<GroupUserEntity, UserTableModel> baseSpecification = new PaginationSpecification<GroupUserEntity, UserTableModel>();
            baseSpecification.AddFilter(x => x.RoleId == roleId);
            baseSpecification.AddSelect(x => new UserTableModel(
                x.User.Id,
                x.User.UserName,
                x.Group.Name));

            if (!string.IsNullOrEmpty(request.Search))
            {
                string search = request.Search.ToUpper();

                baseSpecification.AddFilter(x =>
                    x.User.Id.ToUpper().Contains(search)
                    || x.User.Email.ToUpper().Contains(search)
                    || x.User.UserName.ToUpper().Contains(search)
                    || x.User.FirstName.ToUpper().Contains(search)
                    || x.User.LastName.ToUpper().Contains(search));
            }
            baseSpecification.AppalyPaging(request.Start, request.Length);
            baseSpecification.AddInclude(x => x.User);

            PaginatedData<UserTableModel> paginationData = _groupUserRepository.GetPaginated(baseSpecification);

            DataTableResult<UserTableModel> result = new DataTableResult<UserTableModel>(
                draw: request.Draw,
                recordsTotal: paginationData.Count,
                recordsFilterd: paginationData.Count,
                error: null,
                data: paginationData.Data);

            return Result.Ok(result);
        }

        public Result<RoleMenuViewModel> GetRoleMenuViewModel(string id)
        {
            SelectSpecification<RoleEntity, RoleMenuViewModel> roleSpecification = new SelectSpecification<RoleEntity, RoleMenuViewModel>();
            roleSpecification.AddFilter(x => x.Id == id);
            roleSpecification.AddSelect(x => new RoleMenuViewModel(
                x.Id,
                x.Name,
                x.Type));

            RoleMenuViewModel roleUser = _roleRepository.SingleOrDefault(roleSpecification);
            if (roleUser == null)
            {
                _logger.LogWarning($"No Role. RoleId {id}");
                return Result.Fail<RoleMenuViewModel>("no_role", "No Role");
            }

            return Result.Ok(roleUser);
        }
    }
}
