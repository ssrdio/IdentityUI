using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Admin.Interfaces;
using SSRD.IdentityUI.Admin.Models.Group;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Services
{
    public class GroupUserDataService : IGroupUserDataService
    {
        private readonly IBaseDAO<GroupUserEntity> _groupUserDAO;
        private readonly IBaseDAO<AppUserEntity> _appUserDAO;

        private readonly IValidator<DataTableRequest> _dataTableValidator;
        private readonly IValidator<Select2Request> _select2Validator;

        private readonly ILogger<GroupUserDataService> _logger;

        public GroupUserDataService(
            IBaseDAO<GroupUserEntity> groupUserDAO,
            IBaseDAO<AppUserEntity> appUserDAO,
            IValidator<DataTableRequest> dataTableValidator,
            IValidator<Select2Request> select2Validator,
            ILogger<GroupUserDataService> logger)
        {
            _groupUserDAO = groupUserDAO;
            _appUserDAO = appUserDAO;
            _dataTableValidator = dataTableValidator;
            _select2Validator = select2Validator;
            _logger = logger;
        }

        public async Task<Result<DataTableResult<GroupUserTableModel>>> Get(string groupId, DataTableRequest request)
        {
            ValidationResult validationResult = _dataTableValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid DataTableRequest model");
                return Result.Fail<DataTableResult<GroupUserTableModel>>(validationResult.ToResultError());
            }

            ISelectSpecificationBuilder<GroupUserEntity, GroupUserTableModel> specificationBuilder = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => x.GroupId == groupId)
                .SearchByUsernameEmailId(request.Search)
                .OrderByDessending(x => x._CreatedDate)
                .Select(x => new GroupUserTableModel(
                    x.Id,
                    x.UserId,
                    x.User.UserName,
                    x.User.Email,
                    x.RoleId,
                    x.Role.Name));

            IBaseSpecification<GroupUserEntity, GroupUserTableModel> countSpecification = specificationBuilder.Build();
            IBaseSpecification<GroupUserEntity, GroupUserTableModel> dataSpecification = specificationBuilder
                .Paginate(request.Start, request.Length)
                .Build();

            int count = await _groupUserDAO.Count(countSpecification);
            List<GroupUserTableModel> groupUsers = await _groupUserDAO.Get(dataSpecification);

            DataTableResult<GroupUserTableModel> dataTableResult = new DataTableResult<GroupUserTableModel>(
                draw: request.Draw,
                recordsTotal: count,
                recordsFiltered: count,
                data: groupUsers);

            return Result.Ok(dataTableResult);
        }

        public async Task<Result<Select2Result<Select2ItemBase>>> GetAvailableUsers(Select2Request select2Request)
        {
            ValidationResult validationResult = _select2Validator.Validate(select2Request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(Select2Request)} model");
                return Result.Fail<Select2Result<Select2ItemBase>>(validationResult.ToResultError());
            }

            IBaseSpecification<AppUserEntity, Select2ItemBase> specification = SpecificationBuilder
                .Create<AppUserEntity>()
                .Where(x => x.Groups.Count == 0)
                .SearchByUsername(select2Request.Term)
                .Select(x => new Select2ItemBase(
                    x.Id,
                    x.UserName))
                .Build();

            List<Select2ItemBase> users = await _appUserDAO.Get(specification);

            Select2Result<Select2ItemBase> select2Result = new Select2Result<Select2ItemBase>(
                results: users);

            return Result.Ok(select2Result);
        }
    }
}
