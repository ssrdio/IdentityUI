using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.User.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.User
{
    internal class InviteService : IInviteService
    {
        private const string FAILED_TO_REMOVE_INVITE = "failed_to_remove_invite";
        public const string INVITE_NOT_FOUND = "invite_not_found";

        private readonly IBaseRepository<AppUserEntity> _userRepository;
        private readonly IBaseRepository<InviteEntity> _inviteRepository;
        private readonly IBaseRepository<RoleEntity> _roleRepository;

        private readonly IBaseDAO<InviteEntity> _inviteDAO;

        private readonly IGroupStore _groupStore;
        private readonly IGroupUserStore _groupUserStore;

        private readonly IEmailService _mailService;

        private readonly IValidator<InviteToGroupRequest> _inviteToGroupRequestValidator;
        private readonly IValidator<InviteRequest> _inviteRequestValidator;

        private readonly ILogger<InviteService> _logger;

        private readonly IdentityUIOptions _identityManagementOptions;
        private readonly IdentityUIEndpoints _identityManagementEndpoints;

        public InviteService(
            IBaseRepository<AppUserEntity> userRepository,
            IBaseRepository<InviteEntity> inviteRepository,
            IBaseRepository<RoleEntity> roleRepository,
            IBaseDAO<InviteEntity> inviteDAO,
            IGroupStore groupStore,
            IGroupUserStore groupUserStore,
            IEmailService mailService,
            IValidator<InviteToGroupRequest> inviteToGroupRequestValidator,
            IValidator<InviteRequest> inviteRequestValidator,
            ILogger<InviteService> logger,
            IOptionsSnapshot<IdentityUIOptions> identityManagementOptions,
            IOptionsSnapshot<IdentityUIEndpoints> identityManagementEndpoints)
        {
            _userRepository = userRepository;
            _inviteRepository = inviteRepository;
            _roleRepository = roleRepository;

            _inviteDAO = inviteDAO;

            _groupStore = groupStore;
            _groupUserStore = groupUserStore;

            _mailService = mailService;

            _inviteToGroupRequestValidator = inviteToGroupRequestValidator;
            _inviteRequestValidator = inviteRequestValidator;

            _logger = logger;

            _identityManagementOptions = identityManagementOptions.Value;
            _identityManagementEndpoints = identityManagementEndpoints.Value;
        }

        public Task<Result> InviteToGroup(string groupId, InviteToGroupRequest inviteToGroupRequest)
        {
            _logger.LogInformation($"Adding new group invite");

            ValidationResult validationResult = _inviteToGroupRequestValidator.Validate(inviteToGroupRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(InviteToGroupRequest)} model");
                return Task.FromResult(Result.Fail(validationResult.Errors));
            }

            Result canInviteResult = CanInvite(inviteToGroupRequest.Email);
            if (canInviteResult.Failure)
            {
                return Task.FromResult(Result.Fail(canInviteResult.Errors));
            }

            Result isGroupInviteValid = IsGroupInviteValid(groupId, inviteToGroupRequest.GroupRoleId);
            if (isGroupInviteValid.Failure)
            {
                return Task.FromResult(Result.Fail(isGroupInviteValid.Errors));
            }

            InviteEntity inviteEntity = new InviteEntity(
                email: inviteToGroupRequest.Email,
                token: StringUtils.GenerateToken(),
                status: Data.Enums.Entity.InviteStatuses.Pending,
                roleId: null,
                groupId: groupId,
                groupRoleId: inviteToGroupRequest.GroupRoleId,
                expiresAt: DateTimeOffset.UtcNow.Add(_identityManagementEndpoints.InviteValidForTimeSpan));

            return AddInvite(inviteEntity);
        }

        public Task<Result> Invite(InviteRequest inviteRequest)
        {
            _logger.LogInformation($"Adding new invite");

            ValidationResult validationResult = _inviteRequestValidator.Validate(inviteRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(InviteToGroupRequest)} model");
                return Task.FromResult(Result.Fail(validationResult.Errors));
            }

            Result canInviteResult = CanInvite(inviteRequest.Email);
            if (canInviteResult.Failure)
            {
                return Task.FromResult(Result.Fail(canInviteResult.Errors));
            }

            if (inviteRequest.RoleId != null)
            {
                Result globalRoleExists = GlobalRoleExists(inviteRequest.RoleId);
                if (globalRoleExists.Failure)
                {
                    return Task.FromResult(Result.Fail(globalRoleExists.Errors));
                }
            }

            if (inviteRequest.GroupId != null)
            {
                Result isGroupInviteValid = IsGroupInviteValid(inviteRequest.GroupId, inviteRequest.GroupRoleId);
                if (isGroupInviteValid.Failure)
                {
                    return Task.FromResult(Result.Fail(isGroupInviteValid.Errors));
                }
            }

            InviteEntity inviteEntity = new InviteEntity(
                email: inviteRequest.Email,
                token: StringUtils.GenerateToken(),
                status: Data.Enums.Entity.InviteStatuses.Pending,
                roleId: inviteRequest.RoleId,
                groupId: inviteRequest.GroupId,
                groupRoleId: inviteRequest.GroupRoleId,
                expiresAt: DateTimeOffset.UtcNow.Add(_identityManagementEndpoints.InviteValidForTimeSpan));

            return AddInvite(inviteEntity);
        }

        private async Task<Result> AddInvite(InviteEntity inviteEntity)
        {
            bool addInvite = _inviteRepository.Add(inviteEntity);
            if (!addInvite)
            {
                _logger.LogError($"Failed to add invite");
                return Result.Fail("failed_to_add_invite", "Failed to add invite");
            }

            _logger.LogInformation($"Invite was added, sending email");

            string callbackUrl = QueryHelpers.AddQueryString($"{_identityManagementOptions.BasePath}{_identityManagementEndpoints.AcceptInvite}", "code", inviteEntity.Token);
            callbackUrl = HtmlEncoder.Default.Encode(callbackUrl);

            Result sendMailResult = await _mailService.SendInvite(inviteEntity.Email, callbackUrl);
            if (sendMailResult.Failure)
            {
                return Result.Fail(sendMailResult.Errors);
            }

            return Result.Ok();
        }

        private Result InviteExits(string email)
        {
            SelectSpecification<InviteEntity, InviteEntity> validInvteExistsSpecification = new SelectSpecification<InviteEntity, InviteEntity>();
            validInvteExistsSpecification.AddFilter(x => x.Email == email);
            validInvteExistsSpecification.AddFilter(x => x.Status == Data.Enums.Entity.InviteStatuses.Pending);
            validInvteExistsSpecification.AddSelect(x => x);

            List<InviteEntity> invites = _inviteRepository.GetList(validInvteExistsSpecification);
            if (invites.Any(x => x.ExpiresAt > DateTimeOffset.UtcNow))
            {
                _logger.LogError($"Valid invite already exists");
                return Result.Fail("invite_exists", "Invite exits");
            }

            return Result.Ok();
        }

        private Result UserExist(string email)
        {
            BaseSpecification<AppUserEntity> userExistSpecification = new BaseSpecification<AppUserEntity>();
            userExistSpecification.AddFilter(x => x.NormalizedEmail == email.ToUpper());

            bool userExist = _userRepository.Exist(userExistSpecification);
            if (userExist)
            {
                _logger.LogError($"User with the same email already exists");
                return Result.Fail("user_with_that_email_already_exist", "User with that email already exists");
            }

            return Result.Ok();
        }

        private Result CanInvite(string email)
        {
            Result inviteExits = InviteExits(email);
            if (inviteExits.Failure)
            {
                return Result.Fail(inviteExits.Errors);
            }

            Result userExistsResult = UserExist(email);
            if (userExistsResult.Failure)
            {
                return Result.Fail(userExistsResult.Errors);
            }

            return Result.Ok();
        }

        private Result GroupRoleExists(string groupRoleId)
        {
            BaseSpecification<RoleEntity> baseSpecification = new BaseSpecification<RoleEntity>();
            baseSpecification.AddFilter(x => x.Id == groupRoleId);
            baseSpecification.AddFilter(x => x.Type == Data.Enums.Entity.RoleTypes.Group);

            bool groupRoleExists = _roleRepository.Exist(baseSpecification);
            if (!groupRoleExists)
            {
                _logger.LogError($"No GroupRole. RoleId {groupRoleId}");
                return Result.Fail("no_group_role", "No GroupRole");
            }

            return Result.Ok();
        }

        private Result GlobalRoleExists(string globalRoleId)
        {
            BaseSpecification<RoleEntity> baseSpecification = new BaseSpecification<RoleEntity>();
            baseSpecification.AddFilter(x => x.Id == globalRoleId);
            baseSpecification.AddFilter(x => x.Type == Data.Enums.Entity.RoleTypes.Global);

            bool globalRoleExists = _roleRepository.Exist(baseSpecification);
            if (!globalRoleExists)
            {
                _logger.LogError($"No GlobalRole. RoleId {globalRoleId}");
                return Result.Fail("no_global_role", "No GlobalRole");
            }

            return Result.Ok();
        }

        private Result IsGroupInviteValid(string groupId, string groupRoleId)
        {
            Result groupExistsResult = _groupStore.Exists(groupId);
            if (groupExistsResult.Failure)
            {
                return Result.Fail(groupExistsResult.Errors);
            }

            Result groupRoleExists = GroupRoleExists(groupRoleId);
            if (groupRoleExists.Failure)
            {
                return Result.Fail(groupRoleExists.Errors);
            }

            List<RoleListData> canAssigneRoles = _groupUserStore.CanAssigneGroupRoles();
            if (!canAssigneRoles.Any(x => x.Id == groupRoleId))
            {
                _logger.LogError($"User has no permission so assign role. GroupRoleId {groupRoleId}");
                return Result.Fail("no_permission", "No Permission");
            }

            return Result.Ok();
        }

        private async Task<CommonUtils.Result.Result> Remove(InviteEntity invite)
        {
            bool removeResult = _inviteRepository.Remove(invite);
            if (!removeResult)
            {
                _logger.LogError($"Failed to remove invite");
                return CommonUtils.Result.Result.Fail(FAILED_TO_REMOVE_INVITE);
            }

            return CommonUtils.Result.Result.Ok();
        }

        [Obsolete("Use Remove(string groupId, string inviteId)")]
        public Result Remove(string id)
        {
            _logger.LogInformation($"Removing Invite. InviteId {id}");

            BaseSpecification<InviteEntity> baseSpecification = new BaseSpecification<InviteEntity>();
            baseSpecification.AddFilter(x => x.Id == id);

            InviteEntity invite = _inviteRepository.SingleOrDefault(baseSpecification);
            if (invite == null)
            {
                _logger.LogError($"No Invite. InviteId {id}");
                return Result.Fail("no_invite", "No Invite");
            }

            Task<CommonUtils.Result.Result[]> result = Task.WhenAll(Remove(invite));

            return result.Result.First().ToOldResult();
        }

        public async Task<CommonUtils.Result.Result> Remove(string groupId, string inviteId)
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
                return CommonUtils.Result.Result.Fail(INVITE_NOT_FOUND);
            }

            return await Remove(invite);
        }
    }
}
