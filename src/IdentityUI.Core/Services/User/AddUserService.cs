using FluentValidation;
using FluentValidation.Results;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.User.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Infrastructure.Data.Repository;

namespace SSRD.IdentityUI.Core.Services.User
{
    internal class AddUserService : IAddUserService
    {
        private readonly UserManager<AppUserEntity> _userManager;
        private readonly SignInManager<AppUserEntity> _signInManager;

        private readonly IEmailConfirmationService _emailService;

        private readonly IBaseRepository<InviteEntity> _inviteRepository;
        private readonly IBaseRepository<GroupEntity> _groupRepository; 
        private readonly IBaseRepository<GroupUserEntity> _groupUserRepository;
        private readonly IBaseRepository<RoleEntity> _roleRepository;

        private readonly IValidator<NewUserRequest> _newUserValidator;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<AcceptInviteRequest> _acceptInviteValidator;
        private readonly IValidator<ExternalLoginRegisterRequest> _externalLoginRequsterRequestValidator;

        private readonly IdentityUIEndpoints _identityUIEndpoints;

        private readonly ILogger<AddUserService> _logger;

        public AddUserService(UserManager<AppUserEntity> userManager, SignInManager<AppUserEntity> signInManager, IValidator<NewUserRequest> newUserValidator,
            IValidator<RegisterRequest> registerValidator, IValidator<AcceptInviteRequest> acceptInviteValidator,
            ILogger<AddUserService> logger, IEmailConfirmationService emailService, IBaseRepository<InviteEntity> inviteRepository,
            IBaseRepository<GroupEntity> groupRepository, IBaseRepository<GroupUserEntity> groupUserRepository,
            IValidator<ExternalLoginRegisterRequest> externalLoginRequsterRequestValidator,
            IOptions<IdentityUIEndpoints> identityUIEndpoints, IBaseRepository<RoleEntity> roleRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;

            _inviteRepository = inviteRepository;
            _groupRepository = groupRepository;
            _groupUserRepository = groupUserRepository;
            _roleRepository = roleRepository;

            _newUserValidator = newUserValidator;
            _registerValidator = registerValidator;
            _acceptInviteValidator = acceptInviteValidator;
            _externalLoginRequsterRequestValidator = externalLoginRequsterRequestValidator;

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
        public async Task<Result<string>> AddUser(NewUserRequest newUserRequest, string adminId)
        {
            ValidationResult validationResult = _newUserValidator.Validate(newUserRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogError($"Invalid NewUserRequest. Admin {adminId}");
                return Result.Fail<string>(ResultUtils.ToResultError(validationResult.Errors));
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
                return Result.Fail<string>(ResultUtils.ToResultError(result.Errors));
            }

            appUser = await _userManager.FindByNameAsync(newUserRequest.UserName);
            if (appUser == null)
            {
                _logger.LogError($"Failed to find new user with UserName {newUserRequest.UserName}. Admin {adminId}");
                return Result.Fail<string>("no_user", "No user");
            }

            return Result.Ok(appUser.Id);
        }

        public async Task<Result> Register(RegisterRequest registerRequest)
        {
            //if(!_identityUIEndpoints.RegisterEnabled)
            {
                _logger.LogError($"User tried to register, but registrations are disabled");
                return Result.Fail("registration_is_not_enabled", "Registration disabled");
            }

            ValidationResult validationResult = _registerValidator.Validate(registerRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogError($"Invalid RegisterRequest");
                return Result.Fail(ResultUtils.ToResultError(validationResult.Errors));
            }

            AppUserEntity appUser = new AppUserEntity(
                userName: registerRequest.Email,
                email: registerRequest.Email,
                firstName: registerRequest.FirstName,
                lastName: registerRequest.LastName,
                emailConfirmed: false,
                enabled: true);

            IdentityResult identityResult = await _userManager.CreateAsync(appUser, registerRequest.Password);
            if (!identityResult.Succeeded)
            {
                _logger.LogError($"Failed to register user");
                return Result.Fail(ResultUtils.ToResultError(identityResult.Errors));
            }

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

            await _emailService.SendVerificationMail(appUser, code);

            return Result.Ok();
        }

        public async Task<Result> AcceptInvite(AcceptInviteRequest acceptInvite)
        {
            ValidationResult validationResult = _acceptInviteValidator.Validate(acceptInvite);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(AcceptInviteRequest)} model");
                return Result.Fail(validationResult.Errors);
            }

            BaseSpecification<InviteEntity> getInviteSpecification = new BaseSpecification<InviteEntity>();
            getInviteSpecification.AddFilter(x => x.Token == acceptInvite.Code);
            getInviteSpecification.AddFilter(x => x.Status == Data.Enums.Entity.InviteStatuses.Pending);

            InviteEntity inviteEntity = _inviteRepository.SingleOrDefault(getInviteSpecification);
            if(inviteEntity == null)
            {
                _logger.LogError($"No Invite. Token {acceptInvite.Code}");
                return Result.Fail("no_invite", "No Invite");
            }

            if(inviteEntity.ExpiresAt < DateTimeOffset.UtcNow)
            {
                _logger.LogError($"Invite has expired");
                return Result.Fail("no_invite", "No Invite");
            }

            AppUserEntity appUser = new AppUserEntity(
                userName: inviteEntity.Email,
                email: inviteEntity.Email,
                firstName: acceptInvite.FirstName,
                lastName: acceptInvite.LastName,
                emailConfirmed: true,
                enabled: true);

            IdentityResult identityResult = await _userManager.CreateAsync(appUser, acceptInvite.Password);
            if (!identityResult.Succeeded)
            {
                _logger.LogError($"Failed to create new user for invite. InviteId {inviteEntity.Id}");
                return Result.Fail(ResultUtils.ToResultError(identityResult.Errors));
            }

            inviteEntity.Update(Data.Enums.Entity.InviteStatuses.Accepted);
            bool updateInvite = _inviteRepository.Update(inviteEntity);
            if(!updateInvite)
            {
                _logger.LogWarning($"Failed to update invite status. InnivteId {inviteEntity.Id}, UserId {appUser.Id}");
            }

            if(inviteEntity.GroupId != null)
            {
                AddToGroup(appUser.Id, inviteEntity.GroupId, inviteEntity.GroupRoleId);
            }

            if(inviteEntity.RoleId != null)
            {
                await AddToGlobalRole(appUser, inviteEntity.RoleId);
            }

            return Result.Ok();
        }

        private Result AddToGroup(string userId, string groupId, string groupRoleId)
        {
            _logger.LogInformation($"Adding user to group. UserId {userId}, GroupId {groupId}");

            BaseSpecification<GroupEntity> groupExistsSpecification = new BaseSpecification<GroupEntity>();
            groupExistsSpecification.AddFilter(x => x.Id == groupId);

            bool groupExists = _groupRepository.Exist(groupExistsSpecification);
            if(!groupExists)
            {
                _logger.LogError($"No Group. GroupId {groupId}");
                return Result.Fail("no_group", "No Group");
            }

            BaseSpecification<RoleEntity> groupRoleExistsSpecification = new BaseSpecification<RoleEntity>();
            groupRoleExistsSpecification.AddFilter(x => x.Id == groupRoleId);
            groupRoleExistsSpecification.AddFilter(x => x.Type == Data.Enums.Entity.RoleTypes.Group);

            bool groupRoleExists = _roleRepository.Exist(groupRoleExistsSpecification);
            if(!groupRoleExists)
            {
                _logger.LogWarning($"No GroupRole, adding GroupUser without GroupRole");
                groupRoleId = null;
            }

            GroupUserEntity groupUser = new GroupUserEntity(
                userId: userId,
                groupId: groupId,
                roleId: groupRoleId);

            bool addGroupUser = _groupUserRepository.Add(groupUser);
            if(!addGroupUser)
            {
                _logger.LogError($"Failed to add GroupUser. GroupId {groupId}, UserId {userId}");
                return Result.Fail("failed_to_add_group_user", "Failed to add GroupUser");
            }

            return Result.Ok();
        }

        private async Task<Result> AddToGlobalRole(AppUserEntity appUser, string roleId)
        {
            _logger.LogInformation($"Adding user to global role. UserId {appUser.Id}, RoleId {roleId}");

            BaseSpecification<RoleEntity> baseSpecification = new BaseSpecification<RoleEntity>();
            baseSpecification.AddFilter(x => x.Id == roleId);
            baseSpecification.AddFilter(x => x.Type == Data.Enums.Entity.RoleTypes.Global);

            RoleEntity role = _roleRepository.SingleOrDefault(baseSpecification);
            if(role == null)
            {
                _logger.LogError($"No GlobalRole. RoleId {roleId}");
                return Result.Fail("no_role", "No Role");
            }

            IdentityResult addResult = await _userManager.AddToRoleAsync(appUser, role.Name);
            if(!addResult.Succeeded)
            {
                _logger.LogError($"Failed to add role. UserId {appUser.Id}, RoleId {roleId}");
                return Result.Fail("failed_to_add_role", "Failed to add role");
            }

            return Result.Ok();
        }

        public async Task<Result> ExternalLoginRequest(ExternalLoginRegisterRequest externalLoginRegisterRequest)
        {
            //if (!_identityUIEndpoints.RegisterEnabled)
            {
                _logger.LogError($"User tried to register, but registrations are disabled");
                return Result.Fail("registration_is_not_enabled", "Registration disabled");
            }

            ValidationResult validationResult = _externalLoginRequsterRequestValidator.Validate(externalLoginRegisterRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(ExternalLoginRegisterRequestValidator)} model");
                return Result.Fail(validationResult.Errors);
            }

            ExternalLoginInfo externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if(externalLoginInfo == null)
            {
                _logger.LogError($"Failed to get external login info.");
                return Result.Fail("failed_to_get_external_login_info", "Failed to get external login info");
            }

            AppUserEntity appUser = new AppUserEntity(
                userName: externalLoginRegisterRequest.Email,
                email: externalLoginRegisterRequest.Email,
                firstName: externalLoginRegisterRequest.FirstName,
                lastName: externalLoginRegisterRequest.LastName,
                emailConfirmed: false,
                enabled: true);

            IdentityResult createUserResult = await _userManager.CreateAsync(appUser);
            if(!createUserResult.Succeeded)
            {
                _logger.LogError($"Failed to create user");
                return Result.Fail(createUserResult.Errors);
            }

            IdentityResult addLoginResult = await _userManager.AddLoginAsync(appUser, externalLoginInfo);
            if(!addLoginResult.Succeeded)
            {
                _logger.LogError($"Failed to add login to user. UserId {appUser.Id}");
            }

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

            await _emailService.SendVerificationMail(appUser, code);

            return Result.Ok();
        }
    }
}
