using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Group;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Group;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.Group
{
    internal class GroupDataService : IGroupDataService
    {
        private readonly IBaseRepository<GroupEntity> _groupRepository;

        private readonly IGroupUserService _groupUserService;

        private readonly IValidator<DataTableRequest> _dataTableValidator;

        private readonly ILogger<GroupDataService> _logger;

        public GroupDataService(IBaseRepository<GroupEntity> groupRepository, IGroupUserService groupUserService,
            IValidator<DataTableRequest> dataTableValidator, ILogger<GroupDataService> logger)
        {
            _groupRepository = groupRepository;

            _groupUserService = groupUserService;

            _dataTableValidator = dataTableValidator;

            _logger = logger;
        }

        public Result<DataTableResult<GroupTableModel>> Get(DataTableRequest dataTableRequest)
        {
            ValidationResult validationResult = _dataTableValidator.Validate(dataTableRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid DataTableRequest model");
                return Result.Fail<DataTableResult<GroupTableModel>>(validationResult.Errors);
            }

            PaginationSpecification<GroupEntity, GroupTableModel> paginationSpecification = new PaginationSpecification<GroupEntity, GroupTableModel>();
            if (!string.IsNullOrEmpty(dataTableRequest.Search))
            {
                paginationSpecification.AddFilter(x => x.Name.ToUpper().Contains(dataTableRequest.Search.ToUpper()));
            }

            paginationSpecification.AddSelect(x => new GroupTableModel(
                x.Id,
                x.Name));
            paginationSpecification.AppalyPaging(dataTableRequest.Start, dataTableRequest.Length);

            PaginatedData<GroupTableModel> paginatedResult = _groupRepository.GetPaginated(paginationSpecification);

            DataTableResult<GroupTableModel> dataTableResult = new DataTableResult<GroupTableModel>(
                draw: dataTableRequest.Draw,
                recordsTotal: paginatedResult.Count,
                recordsFilterd: paginatedResult.Count,
                error: null,
                data: paginatedResult.Data);

            return Result.Ok(dataTableResult);
        }

        public Result<GroupMenuViewModel> GetMenuViewModel(string id)
        {
            _logger.LogInformation($"Getting Group. GroupId {id}");

            SelectSpecification<GroupEntity, GroupMenuViewModel> selectSpecification = new SelectSpecification<GroupEntity, GroupMenuViewModel>();
            selectSpecification.AddFilter(x => x.Id == id);
            selectSpecification.AddSelect(x => new GroupMenuViewModel(
                x.Id,
                x.Name));

            return Get(selectSpecification);
        }

        public Result<GroupUserViewModel> GetGroupUserViewModel(string groupId, string userId, bool hasGlobalPermission)
        {
            _logger.LogInformation($"Getting Group. GroupId {groupId}");

            SelectSpecification<GroupEntity, GroupUserViewModel> selectSpecification = new SelectSpecification<GroupEntity, GroupUserViewModel>();
            selectSpecification.AddFilter(x => x.Id == groupId);
            selectSpecification.AddSelect(x => new GroupUserViewModel(
                x.Id,
                x.Name));

            Result<GroupUserViewModel> getGroupUserResult = Get(selectSpecification);
            if(getGroupUserResult.Failure)
            {
                return getGroupUserResult;
            }

            getGroupUserResult.Value.CanMangeGroupRoles = _groupUserService.CanManageRoles(userId, groupId, hasGlobalPermission);

            return Result.Ok(getGroupUserResult.Value);
        }

        private Result<T> Get<T>(SelectSpecification<GroupEntity, T> selectSpecification)
        {
            T group = _groupRepository.Get(selectSpecification);
            if (group == null)
            {
                _logger.LogError($"No group.");
                return Result.Fail<T>("no_group", "No group");
            }

            return Result.Ok(group);
        }

        private Result GroupExist(string id)
        {
            BaseSpecification<GroupEntity> baseSpecification = new BaseSpecification<GroupEntity>();
            baseSpecification.AddFilter(x => x.Id == id);

            bool groupExist = _groupRepository.Exist(baseSpecification);
            if (!groupExist)
            {
                _logger.LogError($"No group. GroupId {id}");
                return Result.Fail("no_group", "No Group");
            }

            return Result.Ok();
        }
    }
}
