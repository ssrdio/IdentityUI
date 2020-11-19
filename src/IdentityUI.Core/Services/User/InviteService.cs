using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Services.User.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.User
{
    internal class InviteService : IInviteService
    {
        private const string FAILED_TO_REMOVE_INVITE = "failed_to_remove_invite";
        private const string INVITE_NOT_FOUND = "invite_not_found";
        private const string FAILED_TO_ADD_INVITE = "failed_to_add_invite";
        private const string NO_PERMISSION = "no_permission";

        private const string INVITE_ALREADY_EXISTS = "invite_already_exists";
        private const string USER_WITH_SAME_EMAIL_ALREADY_EXIST = "user_with_same_email_already_exist";
        private const string GROUP_ROLE_NOT_FOUND = "group_role_not_found";
        private const string GLOBAL_ROLE_NOT_FOUND = "global_role_not_found";

        protected readonly IBaseDAO<AppUserEntity> _userDAO;
        protected readonly IBaseDAO<RoleEntity> _roleDAO;
        protected readonly IBaseDAO<InviteEntity> _inviteDAO;

        protected readonly IGroupStore _groupStore;
        protected readonly IGroupUserStore _groupUserStore;
        protected readonly IEmailService _mailService;
        protected readonly IAddInviteFilter _addInviteFilter;

        protected readonly IValidator<InviteToGroupRequest> _inviteToGroupRequestValidator;
        protected readonly IValidator<InviteRequest> _inviteRequestValidator;

        protected readonly ILogger<InviteService> _logger;

        protected readonly IdentityUIOptions _identityManagementOptions;
        protected readonly IdentityUIEndpoints _identityManagementEndpoints;

        public InviteService(
            IBaseDAO<AppUserEntity> userDAO,
            IBaseDAO<RoleEntity> roleDAO,
            IBaseDAO<InviteEntity> inviteDAO,
            IGroupStore groupStore,
            IGroupUserStore groupUserStore,
            IEmailService mailService,
            IAddInviteFilter addInviteFilter,
            IValidator<InviteToGroupRequest> inviteToGroupRequestValidator,
            IValidator<InviteRequest> inviteRequestValidator,
            ILogger<InviteService> logger,
            IOptions<IdentityUIOptions> identityManagementOptions,
            IOptions<IdentityUIEndpoints> identityManagementEndpoints)
        {
            _userDAO = userDAO;
            _roleDAO = roleDAO;
            _inviteDAO = inviteDAO;

            _groupStore = groupStore;
            _groupUserStore = groupUserStore;
            _mailService = mailService;
            _addInviteFilter = addInviteFilter;

            _inviteToGroupRequestValidator = inviteToGroupRequestValidator;
            _inviteRequestValidator = inviteRequestValidator;

            _logger = logger;

            _identityManagementOptions = identityManagementOptions.Value;
            _identityManagementEndpoints = identityManagementEndpoints.Value;
        }

        public async Task<Core.Models.Result.Result> InviteToGroup(string groupId, InviteToGroupRequest inviteToGroupRequest)
        {
            _logger.LogInformation($"Adding new group invite");

            ValidationResult validationResult = _inviteToGroupRequestValidator.Validate(inviteToGroupRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(InviteToGroupRequest)} model");
                return Result.Fail(validationResult.ToResultError()).ToOldResult();
            }

            return (await AddInvite(inviteToGroupRequest.Email, null, groupId, inviteToGroupRequest.GroupRoleId)).ToOldResult();
        }

        public async Task<Core.Models.Result.Result> Invite(InviteRequest inviteRequest)
        {
            _logger.LogInformation($"Adding new invite");

            ValidationResult validationResult = _inviteRequestValidator.Validate(inviteRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(InviteToGroupRequest)} model");
                return Result.Fail(validationResult.ToResultError()).ToOldResult();
            }

            return (await AddInvite(inviteRequest.Email, inviteRequest.RoleId, inviteRequest.GroupId, inviteRequest.GroupRoleId)).ToOldResult();
        }

        public async Task<Result> AddInvite(string email, string roleId = null, string groupId = null, string groupRoleId = null)
        {
            Result inviteAlreadyExistsResult = await InviteAlreadyExits(email);
            if(inviteAlreadyExistsResult.Failure)
            {
                return inviteAlreadyExistsResult;
            }

            //TODO: change method or response to make more sense
            Result userAlreadyExistsResult = await UserAlreadyExist(email);
            if(userAlreadyExistsResult.Failure)
            {
                return userAlreadyExistsResult;
            }

            if(!string.IsNullOrEmpty(roleId))
            {
                Result roleValidResult = await GlobalRoleExists(roleId);
                if(roleValidResult.Failure)
                {
                    return roleValidResult;
                }
            }

            if(!string.IsNullOrEmpty(groupId))
            {
                Result isGroupValid = await IsGroupInviteValid(groupId, groupRoleId);
                if(isGroupValid.Failure)
                {
                    return isGroupValid;
                }
            }

            Result beforeAddResult = await _addInviteFilter.BeforeAdd(email, roleId, groupId, groupRoleId);
            if(beforeAddResult.Failure)
            {
                return beforeAddResult;
            }

            InviteEntity invite = new InviteEntity(
                email: email,
                token: StringUtils.GenerateToken(),
                status: Data.Enums.Entity.InviteStatuses.Pending,
                roleId: roleId,
                groupId: groupId,
                groupRoleId: groupRoleId,
                expiresAt: DateTimeOffset.UtcNow.Add(_identityManagementEndpoints.InviteValidForTimeSpan));

            bool addInvite = await _inviteDAO.Add(invite);
            if (!addInvite)
            {
                _logger.LogError($"Failed to add invite");
                return Result.Fail(FAILED_TO_ADD_INVITE);
            }

            Result afterAddedResult = await _addInviteFilter.AfterAdded(invite);
            if(afterAddedResult.Failure)
            {
                return afterAddedResult;
            }

            _logger.LogInformation($"Invite was added, sending email");

            string callbackUrl = QueryHelpers.AddQueryString($"{_identityManagementOptions.BasePath}{_identityManagementEndpoints.AcceptInvite}", "code", invite.Token);
            callbackUrl = HtmlEncoder.Default.Encode(callbackUrl);

            Core.Models.Result.Result sendMailResult = await _mailService.SendInvite(invite.Email, callbackUrl);
            if (sendMailResult.Failure)
            {
                return sendMailResult.ToNewResult();
            }

            return Result.Ok();
        }

        private async Task<Result> InviteAlreadyExits(string email)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            IBaseSpecification<InviteEntity, InviteEntity> specification = SpecificationBuilder
                .Create<InviteEntity>()
                .Where(x => x.Email.ToUpper() == email.ToUpper())
                .Where(x => x.Status == Data.Enums.Entity.InviteStatuses.Pending)
                .Where(x => x.ExpiresAt > now)
                .Build();

            bool inviteExists = await _inviteDAO.Exist(specification);
            if(inviteExists)
            {
                _logger.LogError($"Valid invite already exists");
                return Result.Fail(INVITE_ALREADY_EXISTS);
            }

            return Result.Ok();
        }

        private async Task<Result> UserAlreadyExist(string email)
        {
            IBaseSpecification<AppUserEntity, AppUserEntity> specification = SpecificationBuilder
                .Create<AppUserEntity>()
                .Where(x => x.NormalizedEmail == email.ToUpper())
                .Build();

            bool userExist = await _userDAO.Exist(specification);
            if (userExist)
            {
                _logger.LogError($"User with the same email already exists");
                //return Result.Fail(USER_WITH_SAME_EMAIL_ALREADY_EXIST); //TODO: dont expose all users to everybody
                return Result.Fail(FAILED_TO_ADD_INVITE);
            }

            return Result.Ok();
        }

        private async Task<Result> GroupRoleExists(string groupRoleId)
        {
            IBaseSpecification<RoleEntity, RoleEntity> specification = SpecificationBuilder
                .Create<RoleEntity>()
                .Where(x => x.Id == groupRoleId)
                .Where(x => x.Type == Data.Enums.Entity.RoleTypes.Group)
                .Build();

            bool groupRoleExists = await _roleDAO.Exist(specification);
            if (!groupRoleExists)
            {
                _logger.LogError($"No GroupRole. RoleId {groupRoleId}");
                return Result.Fail(GROUP_ROLE_NOT_FOUND);
            }

            return Result.Ok();
        }

        public virtual async Task<Result> CanAssigneGroupRole(string roleId)
        {
            List<RoleListData> canAssigneRoles = await _groupUserStore.CanAssigneRoles();
            if (!canAssigneRoles.Any(x => x.Id == roleId))
            {
                _logger.LogError($"User can not assign that GroupRole. GroupRoleId {roleId}");
                return Result.Fail(NO_PERMISSION);
            }

            return Result.Ok();
        }

        private async Task<Result> GlobalRoleExists(string globalRoleId)
        {
            IBaseSpecification<RoleEntity, RoleEntity> specification = SpecificationBuilder
                .Create<RoleEntity>()
                .Where(x => x.Id == globalRoleId)
                .Where(x => x.Type == Data.Enums.Entity.RoleTypes.Global)
                .Build();

            bool globalRoleExists = await _roleDAO.Exist(specification);
            if (!globalRoleExists)
            {
                _logger.LogError($"No GlobalRole. RoleId {globalRoleId}");
                return Result.Fail(GLOBAL_ROLE_NOT_FOUND);
            }

            return Result.Ok();
        }

        private async Task<Result> IsGroupInviteValid(string groupId, string groupRoleId)
        {
            Result groupExistsResult = await _groupStore.Any(groupId);
            if (groupExistsResult.Failure)
            {
                return Result.Fail(groupExistsResult);
            }

            Result groupRoleExists = await GroupRoleExists(groupRoleId);
            if (groupRoleExists.Failure)
            {
                return Result.Fail(groupRoleExists);
            }

            Result canAssigneGroupRoleResult = await CanAssigneGroupRole(groupRoleId);
            if (canAssigneGroupRoleResult.Failure)
            {
                return Result.Fail(canAssigneGroupRoleResult);
            }

            return Result.Ok();
        }

        private async Task<Result> Remove(InviteEntity invite)
        {
            bool removeResult = await _inviteDAO.Remove(invite);
            if (!removeResult)
            {
                _logger.LogError($"Failed to remove invite");
                return Result.Fail(FAILED_TO_REMOVE_INVITE);
            }

            return Result.Ok();
        }

        [Obsolete("Use Remove(string groupId, string inviteId)")]
        public Core.Models.Result.Result Remove(string id)
        {
            _logger.LogInformation($"Removing Invite. InviteId {id}");

            IBaseSpecification<InviteEntity, InviteEntity> specification = SpecificationBuilder
                .Create<InviteEntity>()
                .Where(x => x.Id == id)
                .Build();

            InviteEntity invite = _inviteDAO.SingleOrDefault(specification).Result;
            if (invite == null)
            {
                _logger.LogError($"No Invite. InviteId {id}");
                return Result.Fail(INVITE_NOT_FOUND).ToOldResult();
            }

            return Remove(invite).Result.ToOldResult();
        }

        public async Task<Result> Remove(string groupId, string inviteId)
        {
            IBaseSpecification<InviteEntity, InviteEntity> specification = SpecificationBuilder
                .Create<InviteEntity>()
                .Where(x => x.Id == inviteId)
                .Where(x => x.GroupId == groupId)
                .Build();

            InviteEntity invite = await _inviteDAO.SingleOrDefault(specification);
            if(invite == null)
            {
                _logger.LogError($"Invite not found. InviteId {inviteId}, GroupId {groupId}");
                return Result.Fail(INVITE_NOT_FOUND);
            }

            return await Remove(invite);
        }
    }
}
