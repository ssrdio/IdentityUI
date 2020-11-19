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
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.User;
using System.Linq;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;

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

        protected readonly UserManager<AppUserEntity> _userManager;
        protected readonly SignInManager<AppUserEntity> _signInManager;

        protected readonly IEmailConfirmationService _emailService;
        protected readonly IGroupUserService _groupUserService;
        protected readonly IAddUserFilter _addUserFilter;

        protected readonly IBaseDAO<AppUserEntity> _userDAO;
        protected readonly IBaseDAO<InviteEntity> _inviteDAO;
        protected readonly IBaseDAO<GroupEntity> _groupDAO;
        protected readonly IBaseDAO<GroupUserEntity> _groupUserDAO;
        protected readonly IBaseDAO<RoleEntity> _roleDAO;

        protected readonly IValidator<NewUserRequest> _newUserValidator;
        protected readonly IValidator<RegisterRequest> _registerValidator;
        protected readonly IValidator<AcceptInviteRequest> _acceptInviteValidator;
        protected readonly IValidator<ExternalLoginRegisterRequest> _externalLoginRequsterRequestValidator;
        protected readonly IValidator<GroupBaseUserRegisterRequest> _groupBaseUserRegisterRequestValidator;
        protected readonly IValidator<BaseRegisterRequest> _baseRegisterValidator;
        protected readonly IValidator<IUserAttributeRequest> _userAttributeRequestValidator;

        protected readonly IdentityUIEndpoints _identityUIEndpoints;

        protected readonly ILogger<AddUserService> _logger;

        public AddUserService(
            UserManager<AppUserEntity> userManager,
            SignInManager<AppUserEntity> signInManager,
            IEmailConfirmationService emailService,
            IGroupUserService groupUserService,
            IAddUserFilter addUserFilter,
            IBaseDAO<AppUserEntity> userDAO,
            IBaseDAO<InviteEntity> inviteDAO,
            IBaseDAO<RoleEntity> roleDAO,
            IOptions<IdentityUIEndpoints> identityUIEndpoints,
            IValidator<NewUserRequest> newUserValidator,
            IValidator<RegisterRequest> registerValidator,
            IValidator<AcceptInviteRequest> acceptInviteValidator,
            IValidator<ExternalLoginRegisterRequest> externalLoginRequsterRequestValidator,
            IValidator<GroupBaseUserRegisterRequest> groupBaseUserRegisterRequestValidator,
            IValidator<BaseRegisterRequest> baseRegisterValidator,
            IValidator<IUserAttributeRequest> userAttributeRequestValidator,
            ILogger<AddUserService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;

            _userDAO = userDAO;
            _inviteDAO = inviteDAO;
            _roleDAO = roleDAO;

            _emailService = emailService;
            _groupUserService = groupUserService;
            _addUserFilter = addUserFilter;

            _newUserValidator = newUserValidator;
            _registerValidator = registerValidator;
            _acceptInviteValidator = acceptInviteValidator;
            _externalLoginRequsterRequestValidator = externalLoginRequsterRequestValidator;
            _groupBaseUserRegisterRequestValidator = groupBaseUserRegisterRequestValidator;
            _userAttributeRequestValidator = userAttributeRequestValidator;
            _baseRegisterValidator = baseRegisterValidator;

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

            BaseRegisterRequest baseRegisterRequest = new BaseRegisterRequest
            {
                Username = newUserRequest.UserName,
                Email = newUserRequest.Email,
                FirstName = newUserRequest.FirstName,
                LastName = newUserRequest.LastName,
            };

            Result<AppUserEntity> addUserResult = await AddUser(baseRegisterRequest, false, false, false);
            if(addUserResult.Failure)
            {
                return Core.Models.Result.Result.Fail<string>(addUserResult.ResultMessages.Select(x => new Core.Models.Result.Result.ResultError(x.Code, x.Code)).ToList());
            }

            return Core.Models.Result.Result.Ok(addUserResult.Value.Id);
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

            if (inviteEntity.GroupId != null)
            {
                Result groupValidResult = await _groupUserService.ValidateGroup(inviteEntity.GroupId);
                if (groupValidResult.Failure)
                {
                    return Result.Fail(groupValidResult).ToOldResult();
                }

                Result groupRoleValidResult = await _groupUserService.RoleIsValid(inviteEntity.GroupRoleId);
                if (groupRoleValidResult.Failure)
                {
                    return Result.Fail(groupRoleValidResult).ToOldResult();
                }
            }

            acceptInvite.Email = inviteEntity.Email;

            Result<AppUserEntity> addUserResult = await AddUser(acceptInvite, sendConfirmationMail: false, emailConfirmed: true);
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
                Result addToGroupResult = await _groupUserService.AddUserToGroupWithoutValidation(appUser.Id, inviteEntity.GroupId, inviteEntity.GroupRoleId);
            }

            if(inviteEntity.RoleId != null)
            {
                Result addToGlobalRole = await AddToGlobalRole(appUser, inviteEntity.RoleId);
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

            //TODO: do not confirm. don't require confirmed emails for external login users
            CommonUtils.Result.Result<AppUserEntity> addUserResult = await AddUser(
                externalLoginRegisterRequest,
                setPassword: false,
                sendConfirmationMail: false,
                emailConfirmed: true);
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

        public virtual async Task<CommonUtils.Result.Result<AppUserEntity>> AddUser(
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

            if(setPassword)
            {
                ValidationResult setPasswordValidationResult = _baseRegisterValidator.Validate(baseRegisterRequest, ruleSet: BaseRegisterRequestValidator.REQUIRE_PASSWORD);
                if(!setPasswordValidationResult.IsValid)
                {
                    foreach(ValidationFailure setPasswordError in setPasswordValidationResult.Errors)
                    {
                        validationResult.Errors.Add(setPasswordError);
                    }
                }
            }

            if (!validationResult.IsValid)
            {
                _logger.LogError($"Invalid {typeof(BaseRegisterRequest).Name} model");
                return Result.Fail<AppUserEntity>(validationResult.ToResultError());
            }

            ValidationResult userAttributeValidationResult = _userAttributeRequestValidator.Validate(baseRegisterRequest);
            if (!userAttributeValidationResult.IsValid)
            {
                _logger.LogError($"Invalid {typeof(IUserAttributeRequest).Name} type");
                return Result.Fail<AppUserEntity>(validationResult.ToResultError());
            }

            List<UserAttributeEntity> userAttributes = null;
            if (baseRegisterRequest.Attributes != null)
            {
                userAttributes = baseRegisterRequest.Attributes
                    .Select(x => new UserAttributeEntity(
                        key: x.Key?.Trim(),
                        value: x.Value?.Trim()))
                    .ToList();
            }

            if (_identityUIEndpoints.UseEmailAsUsername)
            {
                baseRegisterRequest.Username = baseRegisterRequest.Email;
            }

            baseRegisterRequest.Username.Trim();

            Result beforeUserAddResult = await _addUserFilter.BeforeAdd(baseRegisterRequest);
            if(beforeUserAddResult.Failure)
            {
                return Result.Fail<AppUserEntity>(beforeUserAddResult);
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
                return Result.Fail<AppUserEntity>(identityResult.ToResultError());
            }

            Result afterUserAddedResult = await _addUserFilter.AfterAdded(appUser);
            if(afterUserAddedResult.Failure)
            {
                return Result.Fail<AppUserEntity>(afterUserAddedResult);
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
