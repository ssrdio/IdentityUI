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
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.Group
{
    internal class GroupUserDataService : IGroupUserDataService
    {
        private readonly IBaseRepository<AppUserEntity> _userRepository;
        private readonly IBaseRepository<GroupUserEntity> _groupUserRepository;

        private readonly IValidator<DataTableRequest> _dataTableValidator;
        private readonly IValidator<Select2Request> _select2Validator;

        private readonly ILogger<GroupUserDataService> _logger;

        [Obsolete("Use SSRD.IdentityUI.Admin.Services.GroupUserDataService")]
        public GroupUserDataService(IBaseRepository<AppUserEntity> userRepository, IBaseRepository<GroupUserEntity> groupUserRepository,
            IValidator<DataTableRequest> dataTableValidator, IValidator<Select2Request> select2Validator,
            ILogger<GroupUserDataService> logger)
        {
            _userRepository = userRepository;
            _groupUserRepository = groupUserRepository;

            _dataTableValidator = dataTableValidator;
            _select2Validator = select2Validator;

            _logger = logger;
        }

        public Result<DataTableResult<GroupUserTableModel>> Get(string id, DataTableRequest dataTableRequest)
        {
            ValidationResult validationResult = _dataTableValidator.Validate(dataTableRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid DataTableRequest model");
                return Result.Fail<DataTableResult<GroupUserTableModel>>(validationResult.Errors);
            }

            PaginationSpecification<GroupUserEntity, GroupUserTableModel> paginationSpecification = new PaginationSpecification<GroupUserEntity, GroupUserTableModel>();
            paginationSpecification.AddFilter(x => x.GroupId == id);

            if (!string.IsNullOrEmpty(dataTableRequest.Search))
            {
                paginationSpecification.AddFilter(x => x.User.NormalizedUserName.Contains(dataTableRequest.Search.ToUpper()));
            }

            paginationSpecification.AddSelect(x => new GroupUserTableModel(
                x.Id,
                x.User.Id,
                x.User.UserName,
                x.Role.Id,
                x.Role.Name));
            paginationSpecification.AppalyPaging(dataTableRequest.Start, dataTableRequest.Length);

            PaginatedData<GroupUserTableModel> paginatedData = _groupUserRepository.GetPaginated(paginationSpecification);

            DataTableResult<GroupUserTableModel> dataTableResult = new DataTableResult<GroupUserTableModel>(
                draw: dataTableRequest.Draw,
                recordsTotal: paginatedData.Count,
                recordsFilterd: paginatedData.Count,
                error: null,
                data: paginatedData.Data);

            return Result.Ok(dataTableResult);
        }

        public Result<Select2Result<Select2ItemBase>> GetAvailable(Select2Request select2Request)
        {
            ValidationResult validationResult = _select2Validator.Validate(select2Request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(Select2Request)} model");
                return Result.Fail<Select2Result<Select2ItemBase>>(validationResult.Errors);
            }

            SelectSpecification<AppUserEntity, Select2ItemBase> selectSpecification = new SelectSpecification<AppUserEntity, Select2ItemBase>();
            selectSpecification.AddFilter(x => x.Groups.Count == 0); // TODO: change this maybe

            if (!string.IsNullOrEmpty(select2Request.Term))
            {
                selectSpecification.AddFilter(x => x.NormalizedUserName.Contains(select2Request.Term.ToUpper()));
            }

            selectSpecification.AddSelect(x => new Select2ItemBase(
                x.Id,
                x.UserName));

            List<Select2ItemBase> users = _userRepository.GetList(selectSpecification);

            Select2Result<Select2ItemBase> select2Result = new Select2Result<Select2ItemBase>(
                results: users);

            return Result.Ok(select2Result);
        }
    }
}
