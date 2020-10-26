using FluentValidation;
using FluentValidation.Results;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Services.User.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.User;
using System.Linq;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.IdentityUI.Core.Services.User.Models.Add;
using SSRD.IdentityUI.Core.Models;

namespace SSRD.IdentityUI.Core.Services.User
{
    internal class AddUserService : IAddUserService
    {
        public const string USER_NOT_FOUND = "USER_NOT_FOUND";
        public const string GROUP_NOT_FOUND = "group_not_found";
        public const string FAILED_TO_ADD_GROUP_USER = "failed_to_add_group_user";
        public const string ROLE_NO_FOUND = "role_no_found";
        public const string FAILED_TO_ADD_ROLE = "failed_to_add_role";
        public const string INVITE_NOT_FOUND = "invite_not_found";
        public const string REGISTRATION_IS_NOT_ENABLED = "registration_is_not_enabled";

        private readonly UserManager<AppUserEntity> _userManager;
        private readonly SignInManager<AppUserEntity> _signInManager;

        private readonly IEmailConfirmationService _emailService;

        private readonly IBaseDAO<AppUserEntity> _userDAO;
        private readonly IBaseDAO<InviteEntity> _inviteDAO;
        private readonly IBaseDAO<GroupEntity> _groupDAO; 
        private readonly IBaseDAO<GroupUserEntity> _groupUserDAO;
        private readonly IBaseDAO<RoleEntity> _roleDAO;

        private readonly IValidator<NewUserRequest> _newUserValidator;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<AcceptInviteRequest> _acceptInviteValidator;
        private readonly IValidator<ExternalLoginRegisterRequest> _externalLoginRequsterRequestValidator;
        private readonly IValidator<GroupBaseUserRegisterRequest> _groupBaseUserRegisterRequestValidator;
        private readonly IValidator<BaseRegisterRequest> _baseRegisterValidator;
        private readonly IValidator<IUserAttributeRequest> _userAttributeRequestValidator;

        private readonly IdentityUIEndpoints _identityUIEndpoints;

        private readonly ILogger<AddUserService> _logger;

        public AddUserService(
            UserManager<AppUserEntity> userManager,
            SignInManager<AppUserEntity> signInManager,
            IValidator<NewUserRequest> newUserValidator,
            IValidator<RegisterRequest> registerValidator,
            IValidator<AcceptInviteRequest> acceptInviteValidator,
            ILogger<AddUserService> logger,
            IEmailConfirmationService emailService,
            IBaseDAO<AppUserEntity> userDAO,
            IBaseDAO<InviteEntity> inviteDAO,
            IBaseDAO<GroupEntity> groupDAO,
            IBaseDAO<GroupUserEntity> groupUserDAO,
            IBaseDAO<RoleEntity> roleDAO,
            IValidator<ExternalLoginRegisterRequest> externalLoginRequsterRequestValidator,
            IValidator<GroupBaseUserRegisterRequest> groupBaseUserRegisterRequestValidator,
            IValidator<BaseRegisterRequest> baseRegisterValidator,
            IOptions<IdentityUIEndpoints> identityUIEndpoints,
            IValidator<IUserAttributeRequest> userAttributeRequestValidator)
        {
            _userManager = userManager;
            _signInManager = signInManager;

            _userDAO = userDAO;
            _inviteDAO = inviteDAO;
            _groupDAO = groupDAO;
            _groupUserDAO = groupUserDAO;
            _roleDAO = roleDAO;

            _newUserValidator = newUserValidator;
            _registerValidator = registerValidator;
            _acceptInviteValidator = acceptInviteValidator;
            _externalLoginRequsterRequestValidator = externalLoginRequsterRequestValidator;
            _groupBaseUserRegisterRequestValidator = groupBaseUserRegisterRequestValidator;
            _userAttributeRequestValidator = userAttributeRequestValidator;
            _baseRegisterValidator = baseRegisterValidator;

            _emailService = emailService;

            _identityUIEndpoints = identityUIEndpoints.Value;

            _logger = logger;
        }

        /// <summary>
        /// Used only when admin is adding new user
        /// </summary>
        /// <param name="newUserRequest"></param>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public async Task<Core.Models.Result.Result<string>> AddUser(NewUserRequest newUserRequest, string adminId)
        {
            ValidationResult validationResult = _newUserValidator.Validate(newUserRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogError($"Invalid NewUserRequest. Admin {adminId}");
                return Core.Models.Result.Result.Fail<string>(ResultUtils.ToResultError(validationResult.Errors));
            }

            AppUserEntity appUser = new AppUserEntity(
                userName: newUserRequest.UserName,
                email: newUserRequest.Email,
                firstName: newUserRequest.FirstName,
                lastName: newUserRequest.LastName,
                emailConfirmed: false,
                enabled: true);

            IdentityResult result = await _userManager.CreateAsync(appUser);
            if (!result.Succeeded)
            {
                _logger.LogError($"Admin with id {adminId} failed to add new user");
                return Core.Models.Result.Result.Fail<string>(ResultUtils.ToResultError(result.Errors));
            }

            appUser = await _userManager.FindByNameAsync(newUserRequest.UserName);
            if (appUser == null)
            {
                _logger.LogError($"Failed to find new user with UserName {newUserRequest.UserName}. Admin {adminId}");
                return Core.Models.Result.Result.Fail<string>("no_user", "No user");
            }

            return Core.Models.Result.Result.Ok(appUser.Id);
        }

        public async Task<Core.Models.Result.Result> Register(RegisterRequest registerRequest)
        {
            if (!_identityUIEndpoints.RegisterEnabled)
            {
                _logger.LogError($"User tried to register, but registrations are disabled");
                return Core.Models.Result.Result.Fail(REGISTRATION_IS_NOT_ENABLED, REGISTRATION_IS_NOT_ENABLED);
            }

            ValidationResult validationResult = _registerValidator.Validate(registerRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {typeof(RegisterRequest).Name}");
                return Core.Models.Result.Result.Fail(validationResult.Errors);
            }

            CommonUtils.Result.Result<AppUserEntity> addResult = await AddUser(registerRequest);
            if(addResult.Failure)
            {
                return Core.Models.Result.Result.Fail(addResult.ResultMessages.Select(x => new Core.Models.Result.Result.ResultError(x.Code, x.Code, null)).ToList());
            }

            return Core.Models.Result.Result.Ok();
        }

        public async Task<Core.Models.Result.Result> AcceptInvite(AcceptInviteRequest acceptInvite)
        {
            ValidationResult acceptInviteValidationResult = _acceptInviteValidator.Validate(acceptInvite);
            if(!acceptInviteValidationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {typeof(AcceptInviteRequest).Name} model");
            }

            IBaseSpecification<InviteEntity, InviteEntity> getInviteSpecification = SpecificationBuilder
                .Create<InviteEntity>()
                .Where(x => x.Token == acceptInvite.Code)
                .Where(x => x.Status == Data.Enums.Entity.InviteStatuses.Pending)
                .Build();

            InviteEntity inviteEntity = await _inviteDAO.SingleOrDefault(getInviteSpecification);
            if(inviteEntity == null)
            {
                _logger.LogError($"No Invite. Token {acceptInvite.Code}");
                return Core.Models.Result.Result.Fail("no_invite", "No Invite");
            }

            if(inviteEntity.ExpiresAt < DateTimeOffset.UtcNow)
            {
                _logger.LogError($"Invite has expired");
                return Core.Models.Result.Result.Fail("no_invite", "No Invite");
            }

            CommonUtils.Result.Result<AppUserEntity> addUserResult = await AddUser(acceptInvite, sendConfirmationMail: false, emailConfirmed: true);
            if(addUserResult.Failure)
            {
                return addUserResult.ToOldResult();
            }

            AppUserEntity appUser = addUserResult.Value;

            inviteEntity.Update(Data.Enums.Entity.InviteStatuses.Accepted);
            bool updateInvite = await _inviteDAO.Update(inviteEntity);
            if(!updateInvite)
            {
                _logger.LogWarning($"Failed to update invite status. InnivteId {inviteEntity.Id}, UserId {appUser.Id}");
            }

            if(inviteEntity.GroupId != null)
            {
                CommonUtils.Result.Result addToGroupResult = await AddToGroup(appUser.Id, inviteEntity.GroupId, inviteEntity.GroupRoleId);
            }

            if(inviteEntity.RoleId != null)
            {
                CommonUtils.Result.Result addToGlobalRole = await AddToGlobalRole(appUser, inviteEntity.RoleId);
            }

            return Core.Models.Result.Result.Ok();
        }

        public async Task<Core.Models.Result.Result> ExternalLoginRequest(ExternalLoginRegisterRequest externalLoginRegisterRequest)
        {
            if (!_identityUIEndpoints.RegisterEnabled)
            {
                _logger.LogError($"User tried to register, but registrations are disabled");
                return Core.Models.Result.Result.Fail("registration_is_not_enabled", "Registration disabled");
            }

            ValidationResult externalLoginValidationResult = _externalLoginRequsterRequestValidator.Validate(externalLoginRegisterRequest);
            if(!externalLoginValidationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {typeof(ExternalLoginRegisterRequest)} model");
                return Core.Models.Result.Result.Fail(externalLoginValidationResult.Errors);
            }

            ExternalLoginInfo externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if(externalLoginInfo == null)
            {
                _logger.LogError($"Failed to get external login info.");
                return Core.Models.Result.Result.Fail("failed_to_get_external_login_info", "Failed to get external login info");
            }

            CommonUtils.Result.Result<AppUserEntity> addUserResult = await AddUser(externalLoginRegisterRequest, setPassword: false);
            if(addUserResult.Failure)
            {
                return addUserResult.ToOldResult();
            }

            AppUserEntity appUser = addUserResult.Value;

            IdentityResult addLoginResult = await _userManager.AddLoginAsync(appUser, externalLoginInfo);
            if(!addLoginResult.Succeeded)
            {
                _logger.LogError($"Failed to add login to user. UserId {appUser.Id}");
            }

            return Core.Models.Result.Result.Ok();
        }

        public async Task<CommonUtils.Result.Result> UserExists(BaseRegisterRequest baseRegisterRequest)
        {
            string username = baseRegisterRequest.Username;

            if(_identityUIEndpoints.UseEmailAsUsername)
            {
                username = baseRegisterRequest.Email;
            }

            IBaseSpecification<AppUserEntity, AppUserEntity> specification = SpecificationBuilder
                .Create<AppUserEntity>()
                .WithUsername(username)
                .Build();

            bool userExists = await _userDAO.Exist(specification);
            if(!userExists)
            {
                return CommonUtils.Result.Result.Fail(USER_NOT_FOUND);
            }

            return CommonUtils.Result.Result.Ok();
        }

        private async Task<CommonUtils.Result.Result> AddToGroup(string userId, string groupId, string groupRoleId)
        {
            _logger.LogInformation($"Adding user to group. UserId {userId}, GroupId {groupId}");

            IBaseSpecification<GroupEntity, GroupEntity> groupExistsSpecification = SpecificationBuilder
                .Create<GroupEntity>()
                .Where(x => x.Id == groupId)
                .Build();

            bool groupExists = await _groupDAO.Exist(groupExistsSpecification);
            if (!groupExists)
            {
                _logger.LogError($"No Group. GroupId {groupId}");
                return CommonUtils.Result.Result.Fail(GROUP_NOT_FOUND);
            }

            IBaseSpecification<RoleEntity, RoleEntity> groupRoleExistsSpecification = SpecificationBuilder
                .Create<RoleEntity>()
                .Where(x => x.Id == groupRoleId)
                .Where(x => x.Type == Data.Enums.Entity.RoleTypes.Group)
                .Build();

            bool groupRoleExists = await _roleDAO.Exist(groupRoleExistsSpecification);
            if (!groupRoleExists)
            {
                _logger.LogWarning($"No GroupRole, adding GroupUser without GroupRole");
                groupRoleId = null;
            }

            GroupUserEntity groupUser = new GroupUserEntity(
                userId: userId,
                groupId: groupId,
                roleId: groupRoleId);

            bool addGroupUser = await _groupUserDAO.Add(groupUser);
            if (!addGroupUser)
            {
                _logger.LogError($"Failed to add GroupUser. GroupId {groupId}, UserId {userId}");
                return CommonUtils.Result.Result.Fail(FAILED_TO_ADD_GROUP_USER);
            }

            return CommonUtils.Result.Result.Ok();
        }

        private async Task<CommonUtils.Result.Result> AddToGlobalRole(AppUserEntity appUser, string roleId)
        {
            _logger.LogInformation($"Adding user to global role. UserId {appUser.Id}, RoleId {roleId}");

            IBaseSpecification<RoleEntity, RoleEntity> roleSpecification = SpecificationBuilder
                .Create<RoleEntity>()
                .Where(x => x.Id == roleId)
                .Where(x => x.Type == Data.Enums.Entity.RoleTypes.Global)
                .Build();

            RoleEntity role = await _roleDAO.SingleOrDefault(roleSpecification);
            if (role == null)
            {
                _logger.LogError($"No GlobalRole. RoleId {roleId}");
                return CommonUtils.Result.Result.Fail(ROLE_NO_FOUND);
            }

            IdentityResult addResult = await _userManager.AddToRoleAsync(appUser, role.Name);
            if (!addResult.Succeeded)
            {
                _logger.LogError($"Failed to add role. UserId {appUser.Id}, RoleId {roleId}");
                return CommonUtils.Result.Result.Fail(FAILED_TO_ADD_ROLE);
            }

            return CommonUtils.Result.Result.Ok();
        }

        private async Task<CommonUtils.Result.Result<AppUserEntity>> AddUser(
            BaseRegisterRequest baseRegisterRequest,
            bool setPassword = true,
            bool sendConfirmationMail = true,
            bool emailConfirmed = false)
        {
            ValidationResult validationResult;

            if (_identityUIEndpoints.UseEmailAsUsername)
            {
                validationResult = _baseRegisterValidator.Validate(baseRegisterRequest, ruleSet: BaseRegisterRequestValidator.USE_EMAIL_AS_USERNAME);
            }
            else
            {
                validationResult = _baseRegisterValidator.Validate(baseRegisterRequest, ruleSet: BaseRegisterRequestValidator.REQUIRE_EMAIL_USERNAME);
            }

            if (!validationResult.IsValid)
            {
                _logger.LogError($"Invalid {typeof(BaseRegisterRequest).Name} model");
                return CommonUtils.Result.Result.Fail<AppUserEntity>(validationResult.ToResultError());
            }

            ValidationResult userAttributeValidationResult = _userAttributeRequestValidator.Validate(baseRegisterRequest);
            if (!userAttributeValidationResult.IsValid)
            {
                _logger.LogError($"Invalid {typeof(IUserAttributeRequest).Name} type");
                return CommonUtils.Result.Result.Fail<AppUserEntity>(validationResult.ToResultError());
            }

            List<UserAttributeEntity> userAttributes = null;
            if (baseRegisterRequest.Attributes != null)
            {
                userAttributes = baseRegisterRequest.Attributes
                    .Select(x => new UserAttributeEntity(
                        key: x.Key,
                        value: x.Value))
                    .ToList();
            }

            if (_identityUIEndpoints.UseEmailAsUsername)
            {
                baseRegisterRequest.Username = baseRegisterRequest.Email;
            }

            AppUserEntity appUser = new AppUserEntity(
                userName: baseRegisterRequest.Username,
                email: baseRegisterRequest.Email,
                firstName: baseRegisterRequest.FirstName,
                lastName: baseRegisterRequest.LastName,
                emailConfirmed: emailConfirmed,
                enabled: true,
                phoneNumber: baseRegisterRequest.PhoneNumber,
                attributes: userAttributes);

            IdentityResult identityResult;
            if (setPassword)
            {
                identityResult = await _userManager.CreateAsync(appUser, baseRegisterRequest.Password);
            }
            else
            {
                identityResult = await _userManager.CreateAsync(appUser);
            }

            if (!identityResult.Succeeded)
            {
                _logger.LogError($"Failed to register user");
                return CommonUtils.Result.Result.Fail<AppUserEntity>(identityResult.ToResultError());
            }

            if (sendConfirmationMail)
            {
                string code = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

                await _emailService.SendVerificationMail(appUser, code);
            }

            return Result.Ok(appUser);
        }

        public async Task<Result<IdStringModel>> RegisterForGroup(GroupBaseUserRegisterRequest registerRequest)
        {
            ValidationResult validationResult = _groupBaseUserRegisterRequestValidator.Validate(registerRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogError($"Invalid {typeof(GroupBaseUserRegisterRequest).Name} model");
                return Result.Fail<IdStringModel>(validationResult.ToResultError());
            }

            Result<AppUserEntity> addUserResult = await AddUser(registerRequest);
            if(addUserResult.Failure)
            {
                return Result.Fail<IdStringModel>(addUserResult);
            }

            IdStringModel idStringModel = new IdStringModel(
                id: addUserResult.Value.Id);

            return Result.Ok(idStringModel);
        }
    }
}
