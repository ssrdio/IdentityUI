using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Services.Group.Models;
using SSRD.IdentityUI.Core.Services.Role;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Group
{
    public class GroupRegistrationService : IGroupRegistrationService
    {
        public const string USER_ALREADY_EXISTS = "user_already_exists";
        public const string ROLE_NOT_FOUND = "role_not_found";

        private readonly IBaseDAO<RoleEntity> _roleDAO;

        private readonly IGroupService _groupService;
        private readonly IGroupUserService _groupUserService;
        private readonly IAddUserService _addUserService;

        private readonly IValidator<RegisterGroupModel> _registerGroupValidator;
        private readonly ILogger<GroupRegistrationService> _logger;

        public GroupRegistrationService(
            IBaseDAO<RoleEntity> roleDAO,
            IGroupService groupService,
            IGroupUserService groupUserService,
            IAddUserService addUserService,
            IValidator<RegisterGroupModel> registerGroupValidator,
            ILogger<GroupRegistrationService> logger)
        {
            _roleDAO = roleDAO;

            _groupService = groupService;
            _groupUserService = groupUserService;
            _addUserService = addUserService;

            _registerGroupValidator = registerGroupValidator;

            _logger = logger;
        }

        public async Task<Result> Add(RegisterGroupModel registerGroupModel)
        {
            ValidationResult validationResult = _registerGroupValidator.Validate(registerGroupModel);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {typeof(RegisterGroupModel).Name} model");
                return Result.Fail(validationResult.ToResultError());
            }

            Result userExists = await _addUserService.UserExists(registerGroupModel.BaseUser);
            if(userExists.Success)
            {
                _logger.LogError($"User already exists");
                return Result.Fail(USER_ALREADY_EXISTS);
            }

            AddGroupRequest addGroupRequest = new AddGroupRequest()
            {
                Name = registerGroupModel.GroupName
            };

            Result<IdStringModel> addGroupResult = await _groupService.AddAsync(addGroupRequest);
            if(addGroupResult.Failure)
            {
                return Result.Fail(addGroupResult);
            }

            _logger.LogInformation($"Group added. ${addGroupResult.Value.Id}");

            Result<IdStringModel> addUserResult = await _addUserService.RegisterForGroup(registerGroupModel.BaseUser);
            if(addUserResult.Failure)
            {
                _logger.LogError($"Failed to add user. Removing group. GroupId {addGroupResult.Value.Id}");

                //TODO: remove group

                return Result.Fail(addUserResult);
            }

            Result addGroupAdminRoleResult = await AddAdminRole(addUserResult.Value.Id, addGroupResult.Value.Id);
            if(addGroupAdminRoleResult.Failure)
            {
                _logger.LogError($"Failed to add user to group.");

                //TODO: remove group, user

                return Result.Fail(addGroupAdminRoleResult);
            }

            return Result.Ok();
        }

        private async Task<Result> AddAdminRole(string userId, string groupId)
        {
            IBaseSpecification<RoleEntity, RoleEntity> getGroupAdminRoleSpecification = SpecificationBuilder
                .Create<RoleEntity>()
                .Where(x => x.Type == Data.Enums.Entity.RoleTypes.Group)
                .WithName(IdentityUIRoles.GROUP_ADMIN)
                .Build();

            RoleEntity role = await _roleDAO.SingleOrDefault(getGroupAdminRoleSpecification);
            if(role == null)
            {
                _logger.LogError($"GroupAdmin role not found. RoleName {IdentityUIRoles.GROUP_ADMIN}");
                return Result.Fail(ROLE_NOT_FOUND);
            }

            Result result = await _groupUserService.AddUserToGroupWithValidation(userId, groupId, role.Id);
            if(result.Failure)
            {
                return Result.Fail(result);
            }

            return Result.Ok();
        }
    }
}
