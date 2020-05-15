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
using SSRD.IdentityUI.Core.Interfaces;
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

        private readonly IGroupStore _groupStore;
        private readonly IGroupUserStore _groupUserStore;

        private readonly IValidator<DataTableRequest> _dataTableValidator;

        private readonly ILogger<GroupDataService> _logger;

        public GroupDataService(IBaseRepository<GroupEntity> groupRepository, IGroupStore groupStore, IGroupUserStore groupUserStore,
            IValidator<DataTableRequest> dataTableValidator, ILogger<GroupDataService> logger)
        {
            _groupRepository = groupRepository;

            _groupUserStore = groupUserStore;
            _groupStore = groupStore;

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

            GroupMenuViewModel groupMenuView = _groupStore.Get(selectSpecification);
            if(groupMenuView == null)
            {
                _logger.LogError($"No Group. GroupId {id}");
                return Result.Fail<GroupMenuViewModel>("no_group", "No Group");
            }

            return Result.Ok(groupMenuView);
        }

        public Result<GroupUserViewModel> GetGroupUserViewModel(string groupId)
        {
            _logger.LogInformation($"Getting Group. GroupId {groupId}");

            SelectSpecification<GroupEntity, GroupUserViewModel> selectSpecification = new SelectSpecification<GroupEntity, GroupUserViewModel>();
            selectSpecification.AddFilter(x => x.Id == groupId);
            selectSpecification.AddSelect(x => new GroupUserViewModel(
                x.Id,
                x.Name));

            GroupUserViewModel groupUserViewModel = _groupStore.Get(selectSpecification);
            if(groupUserViewModel == null)
            {
                _logger.LogError($"No Group. GroupId {groupId}");
                return Result.Fail<GroupUserViewModel>("no_group", "No Group");
            }

            groupUserViewModel.CanAssigneGroupRoles = _groupUserStore.CanAssigneGroupRoles();
            groupUserViewModel.CanMangeGroupRoles = _groupUserStore.CanManageGroupRoles();
            groupUserViewModel.CanChangeOwnRole = _groupUserStore.CanChangeOwnRole();

            return Result.Ok(groupUserViewModel);
        }

        public Result<GroupInviteViewModel> GetInviteViewModel(string groupId)
        {
            _logger.LogInformation($"Getting Group. GroupId {groupId}");

            SelectSpecification<GroupEntity, GroupInviteViewModel> selectSpecification = new SelectSpecification<GroupEntity, GroupInviteViewModel>();
            selectSpecification.AddFilter(x => x.Id == groupId);
            selectSpecification.AddSelect(x => new GroupInviteViewModel(
                x.Id,
                x.Name));

            GroupInviteViewModel groupInviteView = _groupStore.Get(selectSpecification);
            if (groupInviteView == null)
            {
                _logger.LogError($"No Group. GroupId {groupId}");
                return Result.Fail<GroupInviteViewModel>("no_group", "No Group");
            }

            groupInviteView.CanAssignRoles = _groupUserStore.CanAssigneGroupRoles();

            return Result.Ok(groupInviteView);
        }
    }
}
