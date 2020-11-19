using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Services.Group.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Group
{
    internal class GroupService : IGroupService
    {
        public const string GROUP_WITH_NAME_ALREADY_EXIST = "group_with_name_already_exist";
        public const string FAILED_TO_ADD_GROUP = "failed_to_add_group";
        public const string FAILED_TO_UPDATE_USER = "failed_to_update_user";

        private readonly IBaseDAO<GroupEntity> _groupDAO;
        private readonly IBaseDAO<InviteEntity> _inviteDAO;
        private readonly IBaseDAO<GroupUserEntity> _groupUserDAO;

        private readonly IGroupStore _groupStore;

        private readonly IValidator<AddGroupRequest> _addGroupValidator;
        private readonly IValidator<UpdateGroupModel> _updateGroupModelValidator;

        private readonly ILogger<GroupService> _logger;

        public GroupService(
            IBaseDAO<GroupEntity> groupDAO,
            IBaseDAO<InviteEntity> inviteDAO,
            IBaseDAO<GroupUserEntity> groupUserDAO,
            IGroupStore groupStore,
            IValidator<AddGroupRequest> addGroupValidator,
            IValidator<UpdateGroupModel> updateGroupModelValidator,
            ILogger<GroupService> logger)
        {
            _groupDAO = groupDAO;
            _inviteDAO = inviteDAO;
            _groupUserDAO = groupUserDAO;

            _groupStore = groupStore;

            _addGroupValidator = addGroupValidator;
            _updateGroupModelValidator = updateGroupModelValidator;
            
            _logger = logger;
        }

        public Core.Models.Result.Result Add(AddGroupRequest addGroup)
        {
            Task<Result<IdStringModel>[]> taskResult = Task.WhenAll(AddAsync(addGroup));

            Result result = taskResult.Result.First();

            return result.ToOldResult();
        }

        public async Task<Result<IdStringModel>> AddAsync(AddGroupRequest addGroup)
        {
            ValidationResult validationResult = _addGroupValidator.Validate(addGroup);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid AddGroupRequest model");
                return Result.Fail<IdStringModel>(validationResult.ToResultError());
            }

            addGroup.Name = addGroup.Name.Trim();

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

            return result.ToOldResult();
        }

        public async Task<Result> RemoveAsync(string id)
        {
            IBaseSpecification<GroupEntity, GroupEntity> specification = SpecificationBuilder
                .Create<GroupEntity>()
                .Where(x => x.Id == id)
                .Include(x => x.Invites)
                .Include(x => x.Users)
                .Build();

            Result<GroupEntity> getGroupResult = await _groupStore.SingleOrDefault(specification);
            if(getGroupResult.Failure)
            {
                return Result.Fail(getGroupResult);
            }

            _logger.LogInformation($"Removing group. GroupId {id}");

            GroupEntity groupEntity = getGroupResult.Value;

            string guid = Guid.NewGuid().ToString();

            groupEntity.Name = $"deleted_group_{guid}";

            bool updateResult = await _groupDAO.Update(groupEntity);
            if(!updateResult)
            {
                _logger.LogError($"Failed to update group for deleting. GroupId {id}");
                return Result.Fail(FAILED_TO_UPDATE_USER);
            }

            if(groupEntity.Invites.Any())
            {
                _logger.LogInformation($"Removing group invites. GroupId {groupEntity.Id}");

                bool removeGroupInvitesResult = await _inviteDAO.RemoveRange(groupEntity.Invites);
                if(!removeGroupInvitesResult)
                {
                    _logger.LogError($"Failed to remove group invites. GroupId {groupEntity.Id}");
                }
            }

            if(groupEntity.Users.Any())
            {
                _logger.LogInformation($"Removing group users. GroupId {groupEntity.Id}");

                bool removeGroupUsersResult = await _groupUserDAO.RemoveRange(groupEntity.Users);
                if(!removeGroupUsersResult)
                {
                    _logger.LogError($"Failed to remove group users. GroupId {groupEntity.Id}");
                }
            }

            bool removeResult = await _groupDAO.Remove(groupEntity);
            if(!removeResult)
            {
                _logger.LogError($"Failed to remove Group. GroupId {id}");
                return Result.Fail("failed_to_remove_group", "Failed to remove group");
            }

            return Result.Ok();
        }

        public async Task<Result> Update(string groupId, UpdateGroupModel updateGroupModel)
        {
            ValidationResult validationResult = _updateGroupModelValidator.Validate(updateGroupModel);
            if(!validationResult.IsValid)
            {
                _logger.LogError($"Invalid {typeof(UpdateGroupModel).Name} model");
                return Result.Fail(validationResult.ToResultError());
            }

            Result<GroupEntity> getGroupResult = await _groupStore.SingleOrDefault(groupId);
            if (getGroupResult.Failure)
            {
                return Result.Fail(getGroupResult);
            }

            GroupEntity groupEntity = getGroupResult.Value;

            string guid = Guid.NewGuid().ToString();

            groupEntity.Name = updateGroupModel.GroupName;

            bool updateResult = await _groupDAO.Update(groupEntity);
            if (!updateResult)
            {
                _logger.LogError($"Failed to update group. GroupId {groupId}");
                return Result.Fail(FAILED_TO_UPDATE_USER);
            }

            return Result.Ok();
        }
    }
}
