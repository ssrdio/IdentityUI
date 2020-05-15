using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Group.Models;
using SSRD.IdentityUI.Core.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Group
{
    internal class GroupUserService : IGroupUserService
    {
        private readonly IBaseRepository<GroupUserEntity> _groupUserRepository;
        private readonly IBaseRepository<AppUserEntity> _userRepository;
        private readonly IBaseRepository<RoleEntity> _roleRepository;

        private readonly IGroupUserStore _groupUserStore;
        private readonly IGroupStore _groupStore;

        private readonly IValidator<AddExistingUserRequest> _addExistingUserValidator;

        private readonly ILogger<GroupUserService> _logger;

        public GroupUserService(IBaseRepository<GroupUserEntity> groupUserRepository, IBaseRepository<AppUserEntity> userRepository,
            IBaseRepository<RoleEntity> roleRepository, IGroupUserStore groupUserStore, IGroupStore groupStore,
            IValidator<AddExistingUserRequest> addExistingUserValidator, ILogger<GroupUserService> logger)
        {
            _groupUserRepository = groupUserRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;

            _groupUserStore = groupUserStore;
            _groupStore = groupStore;

            _addExistingUserValidator = addExistingUserValidator;

            _logger = logger;
        }

        private Result AddUserToGroup(string userId, string groupId)
        {
            return AddUserToGroup(userId, groupId, null);
        }

        private Result AddUserToGroup(string userId, string groupId, string roleId)
        {
            Result groupExist = _groupStore.Exists(groupId);
            if (groupExist.Failure)
            {
                return Result.Fail(groupExist.Errors);
            }

            Result roleIsValid = RoleIsValid(roleId);
            if(roleIsValid.Failure)
            {
                return Result.Fail(roleIsValid.Errors);
            }

            Result userExist = ValidateUser(userId);
            if (userExist.Failure)
            {
                return Result.Fail(userExist.Errors);
            }

            GroupUserEntity groupUser = new GroupUserEntity(
                userId: userId,
                groupId: groupId,
                roleId: roleId);

            bool addResult = _groupUserRepository.Add(groupUser);
            if (!addResult)
            {
                _logger.LogError($"Failed to add GroupUser. GroupId {groupId}, UserId {userId}, RoleId {roleId}");
                return Result.Fail("failed_to_add_group_user", "Failed to add GroupUser");
            }

            return Result.Ok();
        }

        private Result ValidateUser(string id)
        {
            Result userExistResult = UserExist(id);
            if(userExistResult.Failure)
            {
                return Result.Fail(userExistResult.Errors);
            }

            BaseSpecification<GroupUserEntity> userAlreadyInGroupSpecification = new BaseSpecification<GroupUserEntity>();
            userAlreadyInGroupSpecification.AddFilter(x => x.UserId == id);

            bool userAlreadyInGroup = _groupUserRepository.Exist(userAlreadyInGroupSpecification);
            if (userAlreadyInGroup)
            {
                _logger.LogError($"User is already in a group. UserId {id}");
                return Result.Fail("user_is_already_in_group", "User is already in a group");
            }

            return Result.Ok();
        }

        private Result RoleIsValid(string roleId)
        {
            if (roleId == null)
            {
                _logger.LogInformation($"Adding GroupUser without role");
                return Result.Ok();
            }

            BaseSpecification<RoleEntity> baseSpecification = new BaseSpecification<RoleEntity>();
            baseSpecification.AddFilter(x => x.Id == roleId);
            baseSpecification.AddFilter(x => x.Type == Data.Enums.Entity.RoleTypes.Group);

            RoleEntity role = _roleRepository.SingleOrDefault(baseSpecification);
            if (role == null)
            {
                _logger.LogError($"No GroupRole. RoleId {roleId}");
                return Result.Fail("no_group_role", "No GroupRole");
            }

            List<RoleListData> canAssigneRoles = _groupUserStore.CanAssigneGroupRoles();
            if (!canAssigneRoles.Any(x => x.Id == roleId))
            {
                _logger.LogError($"User can not assign that GroupRole. GroupRoleId {roleId}");
                return Result.Fail("no_permission", "No permission");
            }

            return Result.Ok();
        }

        private Result UserExist(string id)
        {
            BaseSpecification<AppUserEntity> baseSpecification = new BaseSpecification<AppUserEntity>();
            baseSpecification.AddFilter(x => x.Id == id);

            bool userExist = _userRepository.Exist(baseSpecification);
            if(!userExist)
            {
                _logger.LogError($"No user. UserId {id}");
                return Result.Fail("no_user", "No user");
            }

            return Result.Ok();
        }

        public Result AddExisting(string groupId, AddExistingUserRequest addExistingUserRequest)
        {
            ValidationResult validationResult = _addExistingUserValidator.Validate(addExistingUserRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid AddExistingUserRequest model");
                return Result.Fail(validationResult.Errors);
            }

            return AddUserToGroup(addExistingUserRequest.UserId, groupId, addExistingUserRequest.GroupRoleId);
        }

        public Result ChangeRole(long groupUserId, string roleId, string userId)
        {
            _logger.LogInformation($"Changing GroupUser role. GroupUserId {groupUserId}, roleId {roleId}");

            Result roleValidResult = RoleIsValid(roleId);
            if (roleValidResult.Failure)
            {
                return Result.Fail(roleValidResult.Errors);
            }

            List<RoleListData> canAssigneGroupRoles = _groupUserStore.CanAssigneGroupRoles();
            if(!canAssigneGroupRoles.Any(x => x.Id == roleId))
            {
                _logger.LogError($"User does not have permission to assign role. RoleId {roleId}");
                return Result.Fail("no_permission", "No Permission");
            }

            Result<GroupUserEntity> getGroupUserResult = _groupUserStore.Get(groupUserId);
            if (getGroupUserResult.Failure)
            {
                return Result.Fail(getGroupUserResult.Errors);
            }

            GroupUserEntity groupUser = getGroupUserResult.Value;

            List<RoleListData> canManageGroupRoles = _groupUserStore.CanManageGroupRoles();
            if (!canManageGroupRoles.Any(x => x.Id != groupUser.RoleId))
            {
                _logger.LogError($"User does not have permission to manage role. GroupUserId {groupUserId} RoleId {roleId}");
                return Result.Fail("no_permission", "No Permission");
            }

            if(!_groupUserStore.CanChangeOwnRole())
            {
                if(groupUser.UserId == userId)
                {
                    _logger.LogError($"User can not change his own role");
                    return Result.Fail("user_can_not_change_his_own_role", "User can not change his own role");
                }
            }

            groupUser.UpdateRole(roleId);

            bool updateResult = _groupUserRepository.Update(groupUser);
            if(!updateResult)
            {
                _logger.LogError($"Failed to change group user role. GroupUserId {groupUserId}, roleId {roleId}");
                return Result.Fail("failed_to_cahnge_group_user_role", "Failed to change GroupUser role");
            }

            return Result.Ok();
        }

        public Result Remove(long groupUserId)
        {
            _logger.LogInformation($"Removing GroupUser. GroupUserId {groupUserId}");

            Result<GroupUserEntity> getGroupUserResult = _groupUserStore.Get(groupUserId);
            if (getGroupUserResult.Failure)
            {
                return Result.Fail(getGroupUserResult.Errors);
            }

            GroupUserEntity groupUser = getGroupUserResult.Value;

            List<RoleListData> rolesList = _groupUserStore.CanManageGroupRoles();
            if(!rolesList.Any(x => x.Id == groupUser.RoleId))
            {
                _logger.LogError($"User does not have a permission to remove user. GroupUserId {groupUserId}");
                return Result.Fail("no_permission", "No Permission");
            }

            bool removeResult = _groupUserRepository.Remove(groupUser);
            if(!removeResult)
            {
                _logger.LogError($"Failed to remove GroupUser. GroupUserId {groupUserId}");
                return Result.Fail("failed_to_remove_group_user", "Failed to remove group user");
            }

            return Result.Ok();
        }

        public Result Leave(string userId, string groupId)
        {
            _logger.LogInformation($"GroupUser is leaving. UserId {userId}, GroupId {groupId}");

            Result<GroupUserEntity> getGroupUserResult = _groupUserStore.Get(userId, groupId);
            if(getGroupUserResult.Failure)
            {
                return Result.Fail(getGroupUserResult.Errors);
            }

            GroupUserEntity groupUser = getGroupUserResult.Value;

            bool removeResult = _groupUserRepository.Remove(groupUser);
            if (!removeResult)
            {
                _logger.LogError($"Failed to remove GroupUser. UserId {userId}, GroupId {groupId}");
                return Result.Fail("failed_to_remove_group_user", "Failed to remove group user");
            }

            return Result.Ok();
        }
    }
}
