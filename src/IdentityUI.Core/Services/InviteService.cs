using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services
{
    internal class InviteService : IInviteService
    {
        private readonly IBaseRepository<AppUserEntity> _userRepository;
        private readonly IBaseRepository<GroupEntity> _groupRepository;
        private readonly IBaseRepository<InviteEntity> _inviteRepository;

        private readonly IEmailService _mailService;

        private readonly IValidator<InviteRequest> _inviteValidator;

        private readonly ILogger<InviteService> _logger;

        private readonly IdentityUIOptions _identityManagementOptions;
        private readonly IdentityUIEndpoints _identityManagementEndpoints;

        public InviteService(IBaseRepository<AppUserEntity> userRepository, IBaseRepository<GroupEntity> groupRepository,
            IBaseRepository<InviteEntity> inviteRepository, IEmailService mailService, IValidator<InviteRequest> inviteValidator,
            ILogger<InviteService> logger, IOptionsSnapshot<IdentityUIOptions> identityManagementOptions,
            IOptionsSnapshot<IdentityUIEndpoints> identityManagementEndpoints)
        {
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _inviteRepository = inviteRepository;
            _mailService = mailService;
            _inviteValidator = inviteValidator;
            _logger = logger;

            _identityManagementOptions = identityManagementOptions.Value;
            _identityManagementEndpoints = identityManagementEndpoints.Value;
        }

        public async Task<Result> Invite(InviteRequest inviteRequest)
        {
            _logger.LogInformation($"Adding new invite");

            ValidationResult validationResult = _inviteValidator.Validate(inviteRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(InviteRequest)} model");
                return Result.Fail(validationResult.Errors);
            }

            Result inviteExits = InviteExits(inviteRequest.Email);
            if(inviteExits.Failure)
            {
                return Result.Fail(inviteExits.Errors);
            }

            Result userExistsResult = UserExist(inviteRequest.Email);
            if(userExistsResult.Failure)
            {
                return Result.Fail(userExistsResult.Errors);
            }

            if(inviteRequest.GroupId != null)
            {
                Result groupExistsResult = GroupExists(inviteRequest.GroupId);
            }

            Random random = new Random();
            string token = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 40)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            InviteEntity inviteEntity = new InviteEntity(
                email: inviteRequest.Email,
                token: token,
                status: Data.Enums.Entity.InviteStatuses.Pending,
                groupId: inviteRequest.GroupId,
                expiresAt: DateTimeOffset.UtcNow.Add(_identityManagementEndpoints.InviteValidForTimeSpan));

            bool addInvite = _inviteRepository.Add(inviteEntity);
            if(!addInvite)
            {
                _logger.LogError($"Failed to add invite");
                return Result.Fail("failed_to_add_invite", "Failed to add invite");
            }

            string callbackUrl = QueryHelpers.AddQueryString($"{_identityManagementOptions.BasePath}{_identityManagementEndpoints.AcceptInvite}", "code", token);
            callbackUrl = HtmlEncoder.Default.Encode(callbackUrl);

            Result sendMailResult = await _mailService.SendInvite(inviteEntity.Email, callbackUrl);
            if(sendMailResult.Failure)
            {
                return Result.Fail(sendMailResult.Errors);
            }

            return Result.Ok();
        }

        private Result InviteExits(string email)
        {
            BaseSpecification<InviteEntity> validInvteExistsSpecification = new BaseSpecification<InviteEntity>();
            validInvteExistsSpecification.AddFilter(x => x.Email == email);
            validInvteExistsSpecification.AddFilter(x => x.Status == Data.Enums.Entity.InviteStatuses.Pending);

            InviteEntity invite = _inviteRepository.Get(validInvteExistsSpecification);
            if (invite != null || (invite != null && invite.ExpiresAt < DateTimeOffset.UtcNow))
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
            if(userExist)
            {
                _logger.LogError($"User with the same email already exists");
                return Result.Fail("user_with_that_email_already_exist", "User with that email already exists");
            }

            return Result.Ok();
        }

        private Result GroupExists(string groupId)
        {
            BaseSpecification<GroupEntity> baseSpecification = new BaseSpecification<GroupEntity>();
            baseSpecification.AddFilter(x => x.Id == groupId);

            bool groupExists = _groupRepository.Exist(baseSpecification);
            if(!groupExists)
            {
                _logger.LogError($"No Group. GroupId {groupId}");
                return Result.Fail("no_group", "No Group");
            }

            return Result.Ok();
        }

        public Result Remove(string id)
        {
            _logger.LogInformation($"Removing Invite. InviteId {id}");

            BaseSpecification<InviteEntity> baseSpecification = new BaseSpecification<InviteEntity>();
            baseSpecification.AddFilter(x => x.Id == id);

            InviteEntity invite = _inviteRepository.Get(baseSpecification);
            if(invite == null)
            {
                _logger.LogError($"No Invite. InviteId {id}");
                return Result.Fail("no_invite", "No Invite");
            }

            bool removeResult = _inviteRepository.Remove(invite);
            if(!removeResult)
            {
                _logger.LogError($"Failed to remove invite. InviteId {id}");
                return Result.Fail("failed_to_remove_invite", "Failed to remove invite");
            }

            return Result.Ok();
        }
    }
}
