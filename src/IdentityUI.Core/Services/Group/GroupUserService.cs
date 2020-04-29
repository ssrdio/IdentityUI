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
        private readonly IBaseRepository<GroupEntity> _groupRepository;
        private readonly IBaseRepository<GroupUserEntity> _groupUserRepository;
        private readonly IBaseRepository<AppUserEntity> _userRepository;
        private readonly IBaseRepository<RoleEntity> _roleRepository;
        private readonly IBaseRepository<RoleAssignmentEntity> _roleAssignmenRepository;

        private readonly IValidator<AddExistingUserRequest> _addExistingUserValidator;
        private readonly IValidator<InviteUserToGroupRequest> _inviteUserToGroupRequest;

        private readonly ILogger<GroupUserService> _logger;

        public GroupUserService(IBaseRepository<GroupEntity> groupRepository, IBaseRepository<GroupUserEntity> groupUserRepository,
            IBaseRepository<AppUserEntity> userRepository, IBaseRepository<RoleEntity> roleRepository,
            IBaseRepository<RoleAssignmentEntity> roleAssignmentRepository, IValidator<AddExistingUserRequest> addExistingUserValidator,
            IValidator<InviteUserToGroupRequest> inviteUserToGroupRequest, ILogger<GroupUserService> logger)
        {
            _groupRepository = groupRepository;
            _groupUserRepository = groupUserRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _roleAssignmenRepository = roleAssignmentRepository;

            _addExistingUserValidator = addExistingUserValidator;
            _inviteUserToGroupRequest = inviteUserToGroupRequest;

            _logger = logger;
        }

        private List<RoleListData> GetAllGroupRoles()
        {
            SelectSpecification<RoleEntity, RoleListData> selectSpecification = new SelectSpecification<RoleEntity, RoleListData>();
            selectSpecification.AddFilter(x => x.Type == Data.Enums.Entity.RoleTypes.Group);

            selectSpecification.AddSelect(x => new RoleListData(
                x.Id,
                x.Name));

            List<RoleListData> roleListData = _roleRepository.GetList(selectSpecification);

            return roleListData;
        }

        private List<RoleListData> GetRoleAssignmes(string userId, string groupId)
        {
            SelectSpecification<GroupUserEntity, RoleListData> getGroupRoleSpecification = new SelectSpecification<GroupUserEntity, RoleListData>();
            getGroupRoleSpecification.AddFilter(x => x.UserId == userId);
            getGroupRoleSpecification.AddFilter(x => x.GroupId == groupId);

            getGroupRoleSpecification.AddSelect(x => new RoleListData(
                x.Role.Id,
                x.Role.Name));

            RoleListData groupRole = _groupUserRepository.Get(getGroupRoleSpecification);
            if (groupRole == null)
            {
                _logger.LogInformation($"User has no groupRole. UserId {userId}, GroupId {groupId}");
                return new List<RoleListData>();
            }

            SelectSpecification<RoleAssignmentEntity, RoleListData> getRoleAssignmesSpecification = new SelectSpecification<RoleAssignmentEntity, RoleListData>();
            getRoleAssignmesSpecification.AddFilter(x => x.RoleId == groupRole.Id);

            getRoleAssignmesSpecification.AddSelect(x => new RoleListData(
                x.CanAssigneRole.Id,
                x.CanAssigneRole.Name));

            List<RoleListData> canAssigneRoles = _roleAssignmenRepository.GetList(getRoleAssignmesSpecification);

            if(!canAssigneRoles.Any(x => x.Id == groupRole.Id))
            {
                canAssigneRoles.Add(groupRole);
            }

            return canAssigneRoles;
        }

        public List<RoleListData> CanManageRoles(string userId, string groupId, bool isGlobal)
        {
            List<RoleListData> roles;

            if (isGlobal)
            {
                roles = GetAllGroupRoles();
            }
            else
            {
                roles = GetRoleAssignmes(userId, groupId);
            }

            roles.Add(new RoleListData(
                id: null,
                name: null));

            return roles;
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

        private Result AddUserToGroup(string userId, string groupId)
        {
            //SelectSpecification<GroupRoleEntity, string> defaultGroupRoleSpecification = new SelectSpecification<GroupRoleEntity, string>();
            //defaultGroupRoleSpecification.AddFilter(x => x.Name.ToUpper() == RoleConstats.DEFAULT_GROUP_ROLE.ToUpper());
            //defaultGroupRoleSpecification.AddSelect(x => x.Id);

            //string defaultGroupRoleId = _groupRoleRepository.Get(defaultGroupRoleSpecification);
            //if(string.IsNullOrEmpty(defaultGroupRoleId))
            //{
            //    _logger.LogError($"No default GroupRole. Default GroupRole name {RoleConstats.DEFAULT_GROUP_ROLE}");
            //    return Result.Fail("no_default_group_role", "No default GroupRole");
            //}

            return AddUserToGroup(userId, groupId, null);
        }

        private Result RoleIsValid(string roleId)
        {
            if(roleId == null)
            {
                _logger.LogInformation($"Adding GroupUser without role");
                return Result.Ok();
            }

            BaseSpecification<RoleEntity> baseSpecification = new BaseSpecification<RoleEntity>();
            baseSpecification.AddFilter(x => x.Id == roleId);

            RoleEntity role = _roleRepository.Get(baseSpecification);
            if(role == null)
            {
                _logger.LogError($"No role. RoleId {roleId}");
                return Result.Fail("no_role", "No Role");
            }

            if(role.Type != Data.Enums.Entity.RoleTypes.Group)
            {
                _logger.LogError($"Invalid RoleType. RoleId {roleId}");
                return Result.Fail("invalid_role_type", "Invalid RoleType");
            }

            return Result.Ok();
        }

        private Result AddUserToGroup(string userId, string groupId, string roleId)
        {
            Result groupExist = GroupExist(groupId);
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

            return AddUserToGroup(addExistingUserRequest.UserId, groupId);
        }

        public Task<Result> Invite(string groupId, InviteUserToGroupRequest inviteUserToGroup)
        {
            //ValidationResult validationResult = _inviteUserToGroupRequest.Validate(inviteUserToGroup);
            //if(!validationResult.IsValid)
            //{
            //    _logger.LogWarning($"Invalid {nameof(InviteUserToGroupRequest)} model");
            //    return Result.Fail(validationResult.Errors);
            //}

            throw new NotImplementedException();
        }

        private Result<GroupUserEntity> GetGroupUser(long id)
        {
            BaseSpecification<GroupUserEntity> getGroupUserSpecification = new BaseSpecification<GroupUserEntity>();
            getGroupUserSpecification.AddFilter(x => x.Id == id);

            GroupUserEntity groupUser = _groupUserRepository.Get(getGroupUserSpecification);
            if (groupUser == null)
            {
                _logger.LogError($"No GroupUser. No GroupUser");
                return Result.Fail<GroupUserEntity>("no_group_user", "No GroupUser");
            }

            return Result.Ok(groupUser);
        }

        private Result<GroupUserEntity> GetGroupUser(string userId, string groupId)
        {
            BaseSpecification<GroupUserEntity> getGroupUserSpecification = new BaseSpecification<GroupUserEntity>();
            getGroupUserSpecification.AddFilter(x => x.UserId == userId);
            getGroupUserSpecification.AddFilter(x => x.GroupId == groupId);

            GroupUserEntity groupUser = _groupUserRepository.Get(getGroupUserSpecification);
            if (groupUser == null)
            {
                _logger.LogError($"No GroupUser. No GroupUser");
                return Result.Fail<GroupUserEntity>("no_group_user", "No GroupUser");
            }

            return Result.Ok(groupUser);
        }

        public Result ChangeRole(long groupUserId, string roleId, string logedInUserId, string logedInUserGrroupId, bool hasGlobalPermission)
        {
            _logger.LogInformation($"Changing GroupUser role. GroupUserId {groupUserId}, roleId {roleId}");

            List<RoleListData> roleLists = CanManageRoles(logedInUserId, logedInUserGrroupId, hasGlobalPermission);
            if(!roleLists.Any(x => x.Id == roleId))
            {
                _logger.LogError($"User does not have permission to assign role. RoleId {roleId}");
                return Result.Fail("no_permission", "No Permission");
            }

            Result roleValidResult = RoleIsValid(roleId);
            if(roleValidResult.Failure)
            {
                return Result.Fail(roleValidResult.Errors);
            }

            Result<GroupUserEntity> getGroupUserResult = GetGroupUser(groupUserId);
            if(getGroupUserResult.Failure)
            {
                return Result.Fail(getGroupUserResult.Errors);
            }

            GroupUserEntity groupUser = getGroupUserResult.Value;

            groupUser.UpdateRole(roleId);

            bool updateResult = _groupUserRepository.Update(groupUser);
            if(!updateResult)
            {
                _logger.LogError($"Failed to change group user role. GroupUserId {groupUserId}, roleId {roleId}");
                return Result.Fail("failed_to_cahnge_group_user_role", "Failed to change GroupUser role");
            }

            return Result.Ok();
        }

        public Result Remove(long groupUserId, string loggedInUserId, string loggedInUserGroupId, bool hasGlobalPermissions)
        {
            _logger.LogInformation($"Removing GroupUser. GroupUserId {groupUserId}");

            Result<GroupUserEntity> getGroupUserResult = GetGroupUser(groupUserId);
            if (getGroupUserResult.Failure)
            {
                return Result.Fail(getGroupUserResult.Errors);
            }

            GroupUserEntity groupUser = getGroupUserResult.Value;

            List<RoleListData> rolesList = CanManageRoles(loggedInUserId, loggedInUserGroupId, hasGlobalPermissions);
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

            Result<GroupUserEntity> getGroupUserResult = GetGroupUser(userId, groupId);
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
