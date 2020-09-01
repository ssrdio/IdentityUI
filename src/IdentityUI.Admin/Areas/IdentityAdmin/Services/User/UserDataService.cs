using FluentValidation;
using FluentValidation.Results;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Dashboard;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.User;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Data;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Models.Result;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Models.Options;
using Microsoft.Extensions.Options;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.User;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.User
{
    internal class UserDataService : IUserDataService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IBaseRepository<SessionEntity> _sessionRepository;

        private readonly IdentityUIEndpoints _identityUIEndpoints;

        private readonly IValidator<DataTableRequest> _dataTableValidator;

        private readonly ILogger<UserDataService> _logger;

        public UserDataService(IUserRepository userRepository, IRoleRepository roleRepository, IBaseRepository<SessionEntity> sessionRepository,
            IOptions<IdentityUIEndpoints> identityUIEndpoints, IValidator<DataTableRequest> dataTableValidator, ILogger<UserDataService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _sessionRepository = sessionRepository;

            _identityUIEndpoints = identityUIEndpoints.Value;

            _dataTableValidator = dataTableValidator;

            _logger = logger;
        }

        public Result<DataTableResult<UserListViewModel>> GetAll(DataTableRequest request)
        {
            ValidationResult validationResult = _dataTableValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return Result.Fail<DataTableResult<UserListViewModel>>(ResultUtils.ToResultError(validationResult.Errors.ToList()));
            }

            PaginationSpecification<AppUserEntity, UserListViewModel> baseSpecification = new PaginationSpecification<AppUserEntity, UserListViewModel>();

            if (!string.IsNullOrEmpty(request.Search))
            {
                string search = request.Search.ToUpper();

                baseSpecification.AddFilter(x =>
                    x.Id.ToUpper().Contains(search)
                    || x.Email.ToUpper().Contains(search)
                    || x.UserName.ToUpper().Contains(search)
                    || x.FirstName.ToUpper().Contains(search)
                    || x.LastName.ToUpper().Contains(search));
            }

            baseSpecification.AppalyPaging(request.Start, request.Length);
            baseSpecification.AddSelect(x => new UserListViewModel(
                x.Id,
                x.UserName,
                x.Email,
                x.FirstName,
                x.LastName));

            PaginatedData<UserListViewModel> paginationData = _userRepository.GetPaginated(baseSpecification);

            DataTableResult<UserListViewModel> result = new DataTableResult<UserListViewModel>(
                draw: request.Draw,
                recordsTotal: paginationData.Count,
                recordsFilterd: paginationData.Count,
                error: null,
                data: paginationData.Data);

            return Result.Ok(result);
        }

        public Result<UserCredentialsViewModel> GetCredetialsViewModel(string id)
        {
            SelectSpecification<AppUserEntity, UserCredentialsViewModel> userSpecification = new SelectSpecification<AppUserEntity, UserCredentialsViewModel>();
            userSpecification.AddFilter(x => x.Id == id);
            userSpecification.AddSelect(x => new UserCredentialsViewModel(
                x.Id,
                x.UserName));

            UserCredentialsViewModel userCredentials = _userRepository.SingleOrDefault(userSpecification);
            if (userCredentials == null)
            {
                return Result.Fail<UserCredentialsViewModel>("no_user", "No user");
            }

            return Result.Ok(userCredentials);
        }

        public Result<UserMenuViewModel> GetUserMenuViewModel(string id)
        {
            SelectSpecification<AppUserEntity, UserMenuViewModel> userSpecification = new SelectSpecification<AppUserEntity, UserMenuViewModel>();
            userSpecification.AddFilter(x => x.Id == id);
            userSpecification.AddSelect(x => new UserMenuViewModel(
                x.Id,
                x.UserName));

            UserMenuViewModel userMenuView = _userRepository.SingleOrDefault(userSpecification);
            if (userMenuView == null)
            {
                _logger.LogError($"No User. UserId {id}");
                return Result.Fail<UserMenuViewModel>("no_user", "No user");
            }

            return Result.Ok(userMenuView);
        }

        public Result<UserDetailsViewModel> GetDetailsViewModel(string id)
        {
            SelectSpecification<AppUserEntity, UserDetailsViewModel> userSpecification = new SelectSpecification<AppUserEntity, UserDetailsViewModel>();
            userSpecification.AddFilter(x => x.Id == id);
            userSpecification.AddSelect(x => new UserDetailsViewModel(
                /*id:*/ x.Id,
                /*userName:*/ x.UserName,
                /*email:*/ x.Email,
                /*firstName:*/ x.FirstName,
                /*lastName:*/ x.LastName,
                /*emailConfirmed:*/ x.EmailConfirmed,
                /*phoneNumber:*/ x.PhoneNumber,
                /*phoneNumberConfirmed:*/ x.PhoneNumberConfirmed,
                /*twoFactorEnabled:*/ x.TwoFactorEnabled,
                /*enabled:*/ x.Enabled,
                /*LockoutEnd:*/ x.LockoutEnd.HasValue ? x.LockoutEnd.Value.ToString("d.M.yyyy HH:mm:ss") : null));

            UserDetailsViewModel userDetails = _userRepository.SingleOrDefault(userSpecification);
            if (userDetails == null)
            {
                return Result.Fail<UserDetailsViewModel>("no_user", "No user");
            }

            userDetails.UseEmailSender = _identityUIEndpoints.UseEmailSender ?? false;

            return Result.Ok(userDetails);
        }

        public Result<UserRolesViewModel> GetRolesViewModel(string id)
        {
            SelectSpecification<AppUserEntity, UserRolesViewModel> userSpecification = new SelectSpecification<AppUserEntity, UserRolesViewModel>();
            userSpecification.AddFilter(x => x.Id == id);
            userSpecification.AddSelect(x => new UserRolesViewModel(
                x.Id,
                x.UserName));

            UserRolesViewModel userRoles = _userRepository.SingleOrDefault(userSpecification);
            if (userRoles == null)
            {
                _logger.LogWarning($"No user. UserId {id}");
                return Result.Fail<UserRolesViewModel>("no_user", "No user");
            }

            return Result.Ok(userRoles);
        }

        public Result<RoleViewModel> GetRoles(string id)
        {
            BaseSpecification<AppUserEntity> userSpecification = new BaseSpecification<AppUserEntity>();
            userSpecification.AddFilter(x => x.Id == id);

            bool existResult = _userRepository.Exist(userSpecification);
            if (!existResult)
            {
                _logger.LogWarning($"No user. UserId {id}");
                return Result.Fail<RoleViewModel>("no_user", "No user");
            }

            List<RoleEntity> assignedRoles = _roleRepository.GetAssigned(id);
            List<RoleEntity> avaibleRoles = _roleRepository.GetAvailable(id);

            RoleViewModel roleViewModel = new RoleViewModel(
                assignedRoles: assignedRoles
                    .Select(x => new RoleViewModel.RoleModel(x.Id, x.Name))
                    .OrderBy(x => x.Name)
                    .ToList(),
                availableRoles: avaibleRoles
                    .Select(x => new RoleViewModel.RoleModel(x.Id, x.Name))
                    .OrderBy(x => x.Name)
                    .ToList());

            return Result.Ok(roleViewModel);
        }

        public Result<List<RegistrationsViewModel>> GetRegistrationStatistics(DateTimeOffset from, DateTimeOffset to)
        {
            if(to < from)
            {
                _logger.LogError($"To is smaller than to. From {from.ToString()}, To {to.ToString()}");
                return Result.Fail<List<RegistrationsViewModel>>("error", "Error");
            }

            List<GroupedCountData> countData = _userRepository.GetRegistrations(from, to);

            List<RegistrationsViewModel> viewModel = new List<RegistrationsViewModel>();
            for(DateTimeOffset date = from; date < to; date = date.AddDays(1))
            {
                GroupedCountData registrationData = countData
                    .Where(x => x.Date.Day == date.Day)
                    .Where(x => x.Date.Month == date.Month)
                    .Where(x => x.Date.Year == date.Year)
                    .SingleOrDefault();

                if(registrationData != null)
                {
                    viewModel.Add(new RegistrationsViewModel(
                        date: date.ToString("o"),
                        count: registrationData.Count));
                }
                else
                {
                    viewModel.Add(new RegistrationsViewModel(
                        date: date.ToString("o"),
                        count: 0));
                }
            }

            return Result.Ok(viewModel);
        }

        public Result<DataTableResult<SessionViewModel>> GetSessions(string userId, DataTableRequest request)
        {
            ValidationResult validationResult = _dataTableValidator.Validate(request);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invlid DataTableRequest");
                return Result.Fail<DataTableResult<SessionViewModel>>(validationResult.Errors);
            }

            PaginationSpecification<SessionEntity, SessionViewModel> specification = new PaginationSpecification<SessionEntity, SessionViewModel>();
            specification.AddFilter(x => x.UserId == userId);
            specification.AppalyPaging(request.Start, request.Length);
            specification.AddSelect(x => new SessionViewModel(
                x.Id,
                x.Ip,
                x._CreatedDate,
                x.LastAccess));

            PaginatedData<SessionViewModel> paginatedData = _sessionRepository.GetPaginated(specification);

            DataTableResult<SessionViewModel> dataTableResult = new DataTableResult<SessionViewModel>(
                draw: request.Draw,
                recordsTotal: paginatedData.Count,
                recordsFilterd: paginatedData.Count,
                error: null,
                data: paginatedData.Data);

            return Result.Ok(dataTableResult);
        }

        public Result<UserSessionViewModel> GetUserSessionViewModel(string userId)
        {
            SelectSpecification<AppUserEntity, UserSessionViewModel> userSpecification = new SelectSpecification<AppUserEntity, UserSessionViewModel>();
            userSpecification.AddFilter(x => x.Id == userId);
            userSpecification.AddSelect(x => new UserSessionViewModel(
                x.Id,
                x.UserName));

            UserSessionViewModel userRoles = _userRepository.SingleOrDefault(userSpecification);
            if (userRoles == null)
            {
                _logger.LogWarning($"No user. UserId {userId}");
                return Result.Fail<UserSessionViewModel>("no_user", "No user");
            }

            return Result.Ok(userRoles);
        }

        public StatisticsViewModel GetStatistics()
        {
            int users = _userRepository.GetUsersCount();
            int activeUsers = _userRepository.GetActiveUsersCount();
            int unconfirmedUsers = _userRepository.GetUnconfirmedUsersCount();
            int disabledUsers = _userRepository.GetDisabledUsersCount();

            StatisticsViewModel viewModel = new StatisticsViewModel(
                usersCount: users,
                activeUsersCount: activeUsers,
                unconfirmedUsersCount: unconfirmedUsers,
                disabledUsersCount: disabledUsers);

            return viewModel;
        }
    }
}
