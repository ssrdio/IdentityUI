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

namespace SSRD.IdentityUI.Core.Services.User
{
    internal class AddUserService : IAddUserService
    {
        private readonly UserManager<AppUserEntity> _userManager;

        private readonly IEmailConfirmationService _emailService;

        private readonly IBaseRepository<InviteEntity> _inviteRepository;
        private readonly IBaseRepository<GroupEntity> _groupRepository; 
        private readonly IBaseRepository<GroupUserEntity> _groupUserRepository;

        private readonly IValidator<NewUserRequest> _newUserValidator;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<AcceptInviteRequest> _acceptInviteValidator;

        private readonly ILogger<AddUserService> _logger;

        public AddUserService(UserManager<AppUserEntity> userManager, IValidator<NewUserRequest> newUserValidator,
            IValidator<RegisterRequest> registerValidator, IValidator<AcceptInviteRequest> acceptInviteValidator,
            ILogger<AddUserService> logger, IEmailConfirmationService emailService, IBaseRepository<InviteEntity> inviteRepository,
            IBaseRepository<GroupEntity> groupRepository, IBaseRepository<GroupUserEntity> groupUserRepository)
        {
            _userManager = userManager;

            _inviteRepository = inviteRepository;
            _groupRepository = groupRepository;
            _groupUserRepository = groupUserRepository;

            _newUserValidator = newUserValidator;
            _registerValidator = registerValidator;
            _acceptInviteValidator = acceptInviteValidator;

            _emailService = emailService;

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

            InviteEntity inviteEntity = _inviteRepository.Get(getInviteSpecification);
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
                AddToGroup(appUser.Id, inviteEntity.GroupId);
            }

            return Result.Ok();
        }

        private Result AddToGroup(string userId, string groupId)
        {
            BaseSpecification<GroupEntity> groupExistsSpecification = new BaseSpecification<GroupEntity>();
            groupExistsSpecification.AddFilter(x => x.Id == groupId);

            bool groupExists = _groupRepository.Exist(groupExistsSpecification);
            if(!groupExists)
            {
                _logger.LogError($"No Group. GroupId {groupId}");
                return Result.Fail("no_group", "No Group");
            }

            GroupUserEntity groupUser = new GroupUserEntity(
                userId: userId,
                groupId: groupId,
                roleId: null);

            bool addGroupUser = _groupUserRepository.Add(groupUser);
            if(!addGroupUser)
            {
                _logger.LogError($"Failed to add GroupUser. GroupId {groupId}, UserId {userId}");
                return Result.Fail("failed_to_add_group_user", "Failed to add GroupUser");
            }

            return Result.Ok();
        }
    }
}
