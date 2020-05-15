using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Invite;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
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
        private readonly IBaseRepository<RoleEntity> _roleRepository;
        private readonly IBaseRepository<GroupEntity> _groupRepository;

        private readonly IValidator<DataTableRequest> _dataTableValidator;
        private readonly IValidator<Select2Request> _select2RequestValidator;

        private readonly ILogger<InviteDataService> _logger;

        public InviteDataService(IBaseRepository<InviteEntity> inviteRepository, IBaseRepository<RoleEntity> roleRepository,
            IBaseRepository<GroupEntity> groupRepository, IValidator<DataTableRequest> dataTableValidator,
            IValidator<Select2Request> select2RequestValidator, ILogger<InviteDataService> logger)
        {
            _inviteRepository = inviteRepository;
            _roleRepository = roleRepository;
            _groupRepository = groupRepository;

            _dataTableValidator = dataTableValidator;
            _select2RequestValidator = select2RequestValidator;

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
                x.Role.Name,
                x.Group.Name,
                x.GroupRole.Name,
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

        public Result<Select2Result<Select2ItemBase>> GetGlobalRoles(Select2Request select2Request)
        {
            ValidationResult validationResult = _select2RequestValidator.Validate(select2Request);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(Select2Request)} model");
                return Result.Fail<Select2Result<Select2ItemBase>>(validationResult.Errors);
            }

            SelectSpecification<RoleEntity, Select2ItemBase> selectSpecification =
                new SelectSpecification<RoleEntity, Select2ItemBase>();

            selectSpecification.AddFilter(x => x.Type == Core.Data.Enums.Entity.RoleTypes.Global);

            if(!string.IsNullOrEmpty(select2Request.Term))
            {
                selectSpecification.AddFilter(x => x.NormalizedName.Contains(select2Request.Term.ToUpper()));
            }

            selectSpecification.AddSelect(x => new Select2ItemBase(
                x.Id,
                x.Name));

            List<Select2ItemBase> roles = _roleRepository.GetList(selectSpecification);

            Select2Result<Select2ItemBase> select2Result = new Select2Result<Select2ItemBase>(
                results: roles);

            return Result.Ok(select2Result);
        }

        public Result<Select2Result<Select2ItemBase>> GetGroupRoles(Select2Request select2Request)
        {
            ValidationResult validationResult = _select2RequestValidator.Validate(select2Request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(Select2Request)} model");
                return Result.Fail<Select2Result<Select2ItemBase>>(validationResult.Errors);
            }

            SelectSpecification<RoleEntity, Select2ItemBase> selectSpecification =
                new SelectSpecification<RoleEntity, Select2ItemBase>();

            selectSpecification.AddFilter(x => x.Type == Core.Data.Enums.Entity.RoleTypes.Group);

            if (!string.IsNullOrEmpty(select2Request.Term))
            {
                selectSpecification.AddFilter(x => x.NormalizedName.Contains(select2Request.Term.ToUpper()));
            }

            selectSpecification.AddSelect(x => new Select2ItemBase(
                x.Id,
                x.Name));

            List<Select2ItemBase> roles = _roleRepository.GetList(selectSpecification);

            Select2Result<Select2ItemBase> select2Result = new Select2Result<Select2ItemBase>(
                results: roles);

            return Result.Ok(select2Result);
        }

        public Result<Select2Result<Select2ItemBase>> GetGroups(Select2Request select2Request)
        {
            ValidationResult validationResult = _select2RequestValidator.Validate(select2Request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(Select2Request)} model");
                return Result.Fail<Select2Result<Select2ItemBase>>(validationResult.Errors);
            }

            SelectSpecification<GroupEntity, Select2ItemBase> selectSpecification =
                new SelectSpecification<GroupEntity, Select2ItemBase>();

            if (!string.IsNullOrEmpty(select2Request.Term))
            {
                selectSpecification.AddFilter(x => x.Name.ToUpper().Contains(select2Request.Term.ToUpper()));
            }

            selectSpecification.AddSelect(x => new Select2ItemBase(
                x.Id,
                x.Name));

            List<Select2ItemBase> groups = _groupRepository.GetList(selectSpecification);

            Select2Result<Select2ItemBase> select2Result = new Select2Result<Select2ItemBase>(
                results: groups);

            return Result.Ok(select2Result);
        }
    }
}
