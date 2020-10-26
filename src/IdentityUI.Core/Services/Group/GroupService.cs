using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Interfaces;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Services.Group.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Group
{
    internal class GroupService : IGroupService
    {
        public const string GROUP_WITH_NAME_ALREADY_EXIST = "group_with_name_already_exist";
        public const string FAILED_TO_ADD_GROUP = "failed_to_add_group";

        private readonly IBaseDAO<GroupEntity> _groupDAO;
        private readonly IGroupStore _groupStore;

        private readonly IValidator<AddGroupRequest> _addGroupValidator;

        private readonly ILogger<GroupService> _logger;

        public GroupService(
            IBaseDAO<GroupEntity> groupDAO,
            IGroupStore groupStore,
            IValidator<AddGroupRequest> addGroupValidator,
            ILogger<GroupService> logger)
        {
            _groupDAO = groupDAO;
            _groupStore = groupStore;
            _addGroupValidator = addGroupValidator;
            _logger = logger;
        }

        public Core.Models.Result.Result Add(AddGroupRequest addGroup)
        {
            Task<Result<IdStringModel>[]> taskResult = Task.WhenAll(AddAsync(addGroup));

            Result result = taskResult.Result.First();

            return Core.Models.Result.Result.Fail(result.ResultMessages.Select(x => new Core.Models.Result.Result.ResultError(x.Code, x.Code)).ToList());
        }

        public async Task<Result<IdStringModel>> AddAsync(AddGroupRequest addGroup)
        {
            ValidationResult validationResult = _addGroupValidator.Validate(addGroup);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid AddGroupRequest model");
                return Result.Fail<IdStringModel>(validationResult.ToResultError());
            }

            IBaseSpecification<GroupEntity, GroupEntity> groupExistSpecification = SpecificationBuilder
                .Create<GroupEntity>()
                .WithName(addGroup.Name)
                .Build();

            bool groupExist = await _groupDAO.Exist(groupExistSpecification);
            if(groupExist)
            {
                _logger.LogError($"Group with the same name already exist. GroupName {addGroup.Name}");
                return Result.Fail<IdStringModel>(GROUP_WITH_NAME_ALREADY_EXIST);
            }

            GroupEntity group = new GroupEntity(
                name: addGroup.Name);

            bool addResult = await _groupDAO.Add(group);
            if(!addResult)
            {
                _logger.LogError($"Failed to add group");
                return Result.Fail<IdStringModel>(FAILED_TO_ADD_GROUP);
            }

            IdStringModel idStringModel = new IdStringModel(
                id: group.Id);

            return Result.Ok(idStringModel);
        }

        public Core.Models.Result.Result Remove(string id)
        {
            Task<Result[]> taskResult = Task.WhenAll(RemoveAsync(id));

            Result result = taskResult.Result.First();

            return Core.Models.Result.Result.Fail(result.ResultMessages.Select(x => new Core.Models.Result.Result.ResultError(x.Code, x.Code)).ToList());
        }

        public async Task<Result> RemoveAsync(string id)
        {
            Result<GroupEntity> getGroupResult = await _groupStore.SingleOrDefault(id);
            if(getGroupResult.Failure)
            {
                return Result.Fail(getGroupResult);
            }

            _logger.LogInformation($"Removing group. GroupId {id}");

            GroupEntity groupEntity = getGroupResult.Value;

            bool removeResult = await _groupDAO.Remove(groupEntity);
            if(!removeResult)
            {
                _logger.LogError($"Failed to remove Group. GroupId {id}");
                return Result.Fail("failed_to_remove_group", "Failed to remove group");
            }

            return Result.Ok();
        }
    }
}
