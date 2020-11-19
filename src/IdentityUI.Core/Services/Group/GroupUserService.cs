using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;
using SSRD.IdentityUI.Core.Services.Group.Models;
using SSRD.IdentityUI.Core.Services.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Group
{
    internal class GroupUserService : IGroupUserService
    {
        private const string FAILED_TO_ADD_GROUP_USER = "failed_to_add_group_user";
        private const string USER_NOT_FOUND = "user_not_found";
        private const string GROUP_ROLE_NOT_FOUND = "group_role_not_found";
        private const string GROUP_NOT_FOUND = "group_not_found";
        private const string NO_PERMISSION = "no_permission";
        private const string USER_IS_ALREADY_IN_GROUP = "user_is_already_in_group";
        private const string USER_CAN_NOT_CHANGE_HIS_OWN_ROLE = "user_can_not_change_his_own_role";
        private const string FAILED_TO_CAHNGE_GROUP_USER_ROLE = "failed_to_cahnge_group_user_role";
        private const string FAILED_TO_REMOVE_GROUP_USER = "failed_to_remove_group_user";

        protected readonly IBaseDAO<GroupUserEntity> _groupUserDAO;
        protected readonly IBaseDAO<RoleEntity> _roleDAO;
        protected readonly IBaseDAO<AppUserEntity> _userDAO;
        protected readonly IBaseDAO<GroupEntity> _groupDAO;

        protected readonly IGroupUserStore _groupUserStore;
        protected readonly IAddGroupUserFilter _addGroupUserFilter;

        protected readonly IValidator<AddExistingUserRequest> _addExistingUserValidator;

        protected readonly ILogger<GroupUserService> _logger;

        public GroupUserService(
            IBaseDAO<GroupUserEntity> groupUserDAO,
            IBaseDAO<RoleEntity> roleDAO,
            IBaseDAO<AppUserEntity> userDAO,
            IBaseDAO<GroupEntity> groupDAO,
            IGroupUserStore groupUserStore,
            IAddGroupUserFilter addGroupUserFilter,
            IValidator<AddExistingUserRequest> addExistingUserValidator,
            ILogger<GroupUserService> logger)
        {
            _groupUserStore = groupUserStore;
            _addGroupUserFilter = addGroupUserFilter;

            _groupUserDAO = groupUserDAO;
            _roleDAO = roleDAO;
            _userDAO = userDAO;
            _groupDAO = groupDAO;

            _addExistingUserValidator = addExistingUserValidator;

            _logger = logger;
        }

        public virtual async Task<Result> ValidateGroup(string groupId)
        {
            IBaseSpecification<GroupEntity, GroupEntity> specification = SpecificationBuilder
                .Create<GroupEntity>()
                .WithId(groupId)
                .Build();

            bool exists = await _groupDAO.Exist(specification);
            if(!exists)
            {
                _logger.LogError($"No group. GroupId {groupId}");
                return Result.Fail(GROUP_NOT_FOUND);
            }

            return Result.Ok();
        }

        public virtual async Task<Result> ValidateUser(string id)
        {
            IBaseSpecification<AppUserEntity, AppUserEntity> userExistsSpecification = SpecificationBuilder
                .Create<AppUserEntity>()
                .WithId(id)
                .Build();

            bool userExist = await _userDAO.Exist(userExistsSpecification);
            if (!userExist)
            {
                _logger.LogError($"No user. UserId {id}");
                return Result.Fail(USER_NOT_FOUND);
            }

            IBaseSpecification<GroupUserEntity, GroupUserEntity> userAlreadyInGroupSpecification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => x.UserId == id)
                .Build();

            bool userAlreadyInGroup = await _groupUserDAO.Exist(userAlreadyInGroupSpecification);
            if (userAlreadyInGroup)
            {
                _logger.LogError($"User is already in a group. UserId {id}");
                return Result.Fail(USER_IS_ALREADY_IN_GROUP);
            }

            return Result.Ok();
        }

        public virtual async Task<Result> RoleIsValid(string roleId)
        {
            if (roleId == null)
            {
                _logger.LogInformation($"Adding GroupUser without role");
                return Result.Ok();
            }

            IBaseSpecification<RoleEntity, RoleEntity> specification = SpecificationBuilder
                .Create<RoleEntity>()
                .Where(x => x.Id == roleId)
                .Where(x => x.Type == Data.Enums.Entity.RoleTypes.Group)
                .Build();

            bool roleExists = await _roleDAO.Exist(specification);
            if (!roleExists)
            {
                _logger.LogError($"No GroupRole. RoleId {roleId}");
                return Result.Fail(GROUP_ROLE_NOT_FOUND);
            }

            return Result.Ok();
        }

        public virtual async Task<Result> CanAssigneRole(string roleId)
        {
            List<RoleListData> canAssigneRoles = await _groupUserStore.CanAssigneRoles();
            if (!canAssigneRoles.Any(x => x.Id == roleId))
            {
                _logger.LogError($"User can not assign that GroupRole. GroupRoleId {roleId}");
                return Result.Fail(NO_PERMISSION);
            }

            return Result.Ok();
        }

        public Core.Models.Result.Result AddExisting(string groupId, AddExistingUserRequest addExistingUserRequest)
        {
            ValidationResult validationResult = _addExistingUserValidator.Validate(addExistingUserRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid AddExistingUserRequest model");
                return Core.Models.Result.Result.Fail(validationResult.Errors);
            }

            Task<Result<GroupUserEntity>> addUserToGroup = AddUserToGroupWithValidation(addExistingUserRequest.UserId, groupId, addExistingUserRequest.GroupRoleId);

            Task<Result<GroupUserEntity>[]> taskResult = Task.WhenAll(addUserToGroup);

            Result result = taskResult.Result.Single();

            return result.ToOldResult();
        }

        private async Task<Result> ChangeRoleAsync(long groupUserId, string roleId, string userId)
        {
            _logger.LogInformation($"Changing GroupUser role. GroupUserId {groupUserId}, roleId {roleId}");

            Result roleValidResult = await RoleIsValid(roleId);
            if (roleValidResult.Failure)
            {
                return Result.Fail(roleValidResult);
            }

            List<RoleListData> canAssigneGroupRoles = _groupUserStore.CanAssigneGroupRoles();
            if (!canAssigneGroupRoles.Any(x => x.Id == roleId))
            {
                _logger.LogError($"User does not have permission to assign role. RoleId {roleId}");
                return Result.Fail(NO_PERMISSION);
            }

            Core.Models.Result.Result<GroupUserEntity> getGroupUserResult = _groupUserStore.Get(groupUserId);
            if (getGroupUserResult.Failure)
            {
                return getGroupUserResult.ToNewResult();
            }

            GroupUserEntity groupUser = getGroupUserResult.Value;

            List<RoleListData> canManageGroupRoles = _groupUserStore.CanManageGroupRoles();
            if (!canManageGroupRoles.Any(x => x.Id != groupUser.RoleId))
            {
                _logger.LogError($"User does not have permission to manage role. GroupUserId {groupUserId} RoleId {roleId}");
                return Result.Fail(NO_PERMISSION);
            }

            if (!_groupUserStore.CanChangeOwnRole())
            {
                if (groupUser.UserId == userId)
                {
                    _logger.LogError($"User can not change his own role");
                    return Result.Fail(USER_CAN_NOT_CHANGE_HIS_OWN_ROLE);
                }
            }

            groupUser.UpdateRole(roleId);

            bool updateResult = await _groupUserDAO.Update(groupUser);
            if (!updateResult)
            {
                _logger.LogError($"Failed to change group user role. GroupUserId {groupUserId}, roleId {roleId}");
                return Result.Fail(FAILED_TO_CAHNGE_GROUP_USER_ROLE);
            }

            return Result.Ok();
        }

        public Core.Models.Result.Result ChangeRole(long groupUserId, string roleId, string userId)
        {
            Task<Result> changeRole = ChangeRoleAsync(groupUserId, roleId, userId);

            Task<Result[]> taskResult = Task.WhenAll(changeRole);

            Result result = taskResult.Result.Single();

            return result.ToOldResult();
        }

        public async Task<Result> RemoveAsync(long groupUserId)
        {
            _logger.LogInformation($"Removing GroupUser. GroupUserId {groupUserId}");

            Result<GroupUserEntity> getGroupUserResult = await _groupUserStore.SingleOrDefault(groupUserId);
            if(getGroupUserResult.Failure)
            {
                return Result.Fail(getGroupUserResult);
            }

            GroupUserEntity groupUser = getGroupUserResult.Value;

            Result canManageResult = await _groupUserStore.CanManageUser(groupUser);
            if(canManageResult.Failure)
            {
                return Result.Fail(canManageResult);
            }

            bool removeResult = await _groupUserDAO.Remove(groupUser);
            if(!removeResult)
            {
                _logger.LogError($"Failed to remove GroupUser. GroupUserId {groupUserId}");
                return Result.Fail(FAILED_TO_REMOVE_GROUP_USER);
            }

            return Result.Ok();
        }

        public Core.Models.Result.Result Remove(long groupUserId)
        {
            Task<Result> remove = RemoveAsync(groupUserId);

            Task<Result[]> taskResult = Task.WhenAll(remove);

            Result result = taskResult.Result.Single();

            return result.ToOldResult();
        }

        private async Task<Result> LeaveAsync(string userId, string groupId)
        {
            _logger.LogInformation($"GroupUser is leaving. UserId {userId}, GroupId {groupId}");

            Result<GroupUserEntity> getGroupUserResult = await _groupUserStore.SingleOrDefault(userId, groupId);
            if(getGroupUserResult.Failure)
            {
                return Result.Fail(getGroupUserResult);
            }

            GroupUserEntity groupUser = getGroupUserResult.Value;

            bool removeResult = await _groupUserDAO.Remove(groupUser);
            if (!removeResult)
            {
                _logger.LogError($"Failed to remove GroupUser. UserId {userId}, GroupId {groupId}");
                return Result.Fail("failed_to_remove_group_user", "Failed to remove group user");
            }

            return Result.Ok();
        }

        public Core.Models.Result.Result Leave(string userId, string groupId)
        {
            Task<Result> leave = LeaveAsync(userId, groupId);

            Task<Result[]> taskResult = Task.WhenAll(leave);

            Result result = taskResult.Result.Single();

            return result.ToOldResult();
        }

        /// <summary>
        /// This method validates all parameters
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<Result<GroupUserEntity>> AddUserToGroupWithValidation(string userId, string groupId, string roleId)
        {
            Result groupExist = await ValidateGroup(groupId);
            if (groupExist.Failure)
            {
                return Result.Fail<GroupUserEntity>(groupExist);
            }

            Result roleIsValid = await RoleIsValid(roleId);
            if (roleIsValid.Failure)
            {
                return Result.Fail<GroupUserEntity>(roleIsValid);
            }

            Result canAssigneRole = await CanAssigneRole(roleId);
            if(canAssigneRole.Failure)
            {
                return Result.Fail<GroupUserEntity>(canAssigneRole);
            }

            Result userExist = await ValidateUser(userId);
            if (userExist.Failure)
            {
                return Result.Fail<GroupUserEntity>(userExist);
            }

            return await AddUserToGroupWithoutValidation(userId, groupId, roleId);
        }

        /// <summary>
        /// This method requires that you already validated all parameters
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<Result<GroupUserEntity>> AddUserToGroupWithoutValidation(string userId, string groupId, string roleId)
        {
            Result beforeResult = await _addGroupUserFilter.BeforeAdd(userId, groupId, roleId);
            if(beforeResult.Failure)
            {
                return Result.Fail<GroupUserEntity>(beforeResult);
            }

            GroupUserEntity groupUser = new GroupUserEntity(
                userId: userId,
                groupId: groupId,
                roleId: roleId);

            bool addResult = await _groupUserDAO.Add(groupUser);
            if (!addResult)
            {
                _logger.LogError($"Failed to add GroupUser. GroupId {groupId}, UserId {userId}, RoleId {roleId}");
                return Result.Fail<GroupUserEntity>(FAILED_TO_ADD_GROUP_USER);
            }

            Result afterResult = await _addGroupUserFilter.AfterAdded(groupUser);
            if (afterResult.Failure)
            {
                return Result.Fail<GroupUserEntity>(afterResult);
            }

            return Result.Ok(groupUser);
        }
    }
}
