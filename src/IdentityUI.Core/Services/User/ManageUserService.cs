using FluentValidation;
using FluentValidation.Results;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Data;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.User.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.User
{
    internal class ManageUserService : IManageUserService
    {
        private readonly UserManager<AppUserEntity> _userManager;

        private readonly IUserRepository _userRepository;

        private readonly IEmailConfirmationService _emailConfirmationService;
        private readonly ISessionService _sessionService;

        private readonly IValidator<EditUserRequest> _editUserValidator;
        private readonly IValidator<SetNewPasswordRequest> _setNewPasswordValidator;
        private readonly IValidator<EditProfileRequest> _editProfileValidator;
        private readonly IValidator<UnlockUserRequest> _unlockUserValidator;
        private readonly IValidator<SendEmailVerificationMailRequest> _sendEmailVerificationMailValidator;

        private readonly ILogger<ManageUserService> _logger;

        public ManageUserService(UserManager<AppUserEntity> userManager, IUserRepository userRepository, IEmailConfirmationService emailConfirmationService,
            ISessionService sessionService, IValidator<EditUserRequest> editUserValidator, IValidator<SetNewPasswordRequest> setNewPasswordValidator,
            IValidator<EditProfileRequest> editRequestValidator, IValidator<UnlockUserRequest> unlockUserValidator,
            IValidator<SendEmailVerificationMailRequest> sendEmailVerificationMailValidator, ILogger<ManageUserService> logger)
        {
            _userManager = userManager;

            _userRepository = userRepository;

            _emailConfirmationService = emailConfirmationService;
            _sessionService = sessionService;

            _editUserValidator = editUserValidator;
            _setNewPasswordValidator = setNewPasswordValidator;
            _editProfileValidator = editRequestValidator;

            _unlockUserValidator = unlockUserValidator;
            _sendEmailVerificationMailValidator = sendEmailVerificationMailValidator;

            _logger = logger;
        }

        public async Task<Result> EditUser(string id, EditUserRequest editUserRequest, string adminId)
        {
            ValidationResult validationResult = _editUserValidator.Validate(editUserRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogError($"Invalid EditUserRequest. Admin {adminId}");
                return Result.Fail(ResultUtils.ToResultError(validationResult.Errors));
            }

            BaseSpecification<AppUserEntity> userSpecification = new BaseSpecification<AppUserEntity>();
            userSpecification.AddFilter(x => x.Id == id);

            AppUserEntity appUser = _userRepository.Get(userSpecification);
            if(appUser == null)
            {
                _logger.LogWarning($"No User. UserId {id}. Admin {adminId}");
                return Result.Fail("no_user", "No User");
            }

            bool updateSecurityStamp = false;

            appUser.FirstName = editUserRequest.FirstName;
            appUser.LastName = editUserRequest.LastName;

#if NET_CORE2
            string normalizeEmail = _userManager.NormalizeKey(editUserRequest.Email);
#endif
#if NET_CORE3
            string normalizeEmail = _userManager.NormalizeEmail(editUserRequest.Email);
#endif
            if (normalizeEmail != appUser.NormalizedEmail)
            {
                appUser.Email = editUserRequest.Email;
                appUser.NormalizedEmail = normalizeEmail;
                appUser.EmailConfirmed = false;
            }
            else
            {
                appUser.EmailConfirmed = editUserRequest.EmailConfirmed;
            }

            if (appUser.PhoneNumber != editUserRequest.PhoneNumber)
            {
                appUser.PhoneNumber = editUserRequest.PhoneNumber;
                appUser.PhoneNumberConfirmed = false;
            }
            else
            {
                appUser.PhoneNumberConfirmed = editUserRequest.PhoneNumberConfirmed;
            }

            if(appUser.TwoFactorEnabled)
            {
                appUser.TwoFactorEnabled = editUserRequest.TwoFactorEnabled;
            }

            if (appUser.Enabled != editUserRequest.Enabled)
            {
                appUser.Enabled = editUserRequest.Enabled;
                updateSecurityStamp = true;
            }

            bool result = _userRepository.Update(appUser);
            if(!result)
            {
                _logger.LogError($"Faild to save edited user data. Admin {adminId}");
                return Result.Fail("error", "error");
            }

            if (updateSecurityStamp)
            {
                Result logoutUserResult = await _sessionService.LogoutUser(new Auth.Session.Models.LogoutUserSessionsRequest(appUser.Id), adminId);
                if(logoutUserResult.Failure)
                {
                    return logoutUserResult;
                }
            }

            return Result.Ok();
        }

        public async Task<Result> SetNewPassword(string userId, SetNewPasswordRequest setNewPasswordRequest, string adminId)
        {
            ValidationResult validationResult = _setNewPasswordValidator.Validate(setNewPasswordRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogError($"Invlid SetNewPasswordRequest. Admin {adminId}");
                return Result.Fail(ResultUtils.ToResultError(validationResult.Errors));
            }

            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                _logger.LogError($"No User with id {userId}. Admin {adminId}");
                return Result.Fail("no_user", "No User");
            }

            _logger.LogInformation($"Seting new password for with id {userId}. Admin id {adminId}");

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(appUser);

            IdentityResult changePasswordResult = await _userManager.ResetPasswordAsync(appUser, passwordResetToken, setNewPasswordRequest.Password);
            if (!changePasswordResult.Succeeded)
            {
                _logger.LogError($"Faild to reset password. UserId {appUser.Id}, admin {adminId}");
                return Result.Fail(changePasswordResult.Errors);
            }
            

            Result logoutUserResult = await _sessionService.LogoutUser(new Auth.Session.Models.LogoutUserSessionsRequest(appUser.Id), adminId);
            if (logoutUserResult.Failure)
            {
                return logoutUserResult;
            }

            _logger.LogInformation($"Added new password to user with id {userId}. Admin id {adminId}");
            return Result.Ok();
        }

        public async Task<Result> RemoveRoles(string userId, List<string> roles, string adminId)
        {
            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                _logger.LogError($"No user {userId}");
                return Result.Fail("no_user", "No User");
            }

            IdentityResult result = await _userManager.RemoveFromRolesAsync(appUser, roles);
            if (!result.Succeeded)
            {
                _logger.LogError($"Admin with id {adminId} faild to remove roles to user with id {userId}");
                return Result.Fail(ResultUtils.ToResultError(result.Errors));
            }

            _logger.LogInformation($"Admin with id {adminId} removed roles to user with id {userId}. Role ids: {Newtonsoft.Json.JsonConvert.SerializeObject(roles)}");
            return Result.Ok();
        }

        public async Task<Result> AddRoles(string userId, List<string> roles, string adminId)
        {
            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                _logger.LogError($"No user {userId}");
                return Result.Fail("no_user", "No User");
            }

            IdentityResult result = await _userManager.AddToRolesAsync(appUser, roles);
            if (!result.Succeeded)
            {
                _logger.LogError($"Admin with id {adminId} faild to add roles to user with id {userId}");
                return Result.Fail(ResultUtils.ToResultError(result.Errors));
            }

            _logger.LogInformation($"Admin with id {adminId} added roles to user with id {userId}. Role ids: {Newtonsoft.Json.JsonConvert.SerializeObject(roles)}");
            return Result.Ok();
        }

        public Result EditUser(string id, EditProfileRequest editProfileRequest)
        {
            ValidationResult validationResult = _editProfileValidator.Validate(editProfileRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invlid EditProfileRequest. UserId {id}");
                return Result.Fail(validationResult.Errors);
            }

            BaseSpecification<AppUserEntity> userSpecification = new BaseSpecification<AppUserEntity>();
            userSpecification.AddFilter(x => x.Id == id);

            AppUserEntity appUser = _userRepository.Get(userSpecification);
            if(appUser == null)
            {
                _logger.LogWarning($"No User. UserId {id}");
                return Result.Fail("no_user", "No user");
            }

            appUser.FirstName = editProfileRequest.FirstName;
            appUser.LastName = editProfileRequest.LastName;

            if (appUser.PhoneNumber != editProfileRequest.PhoneNumber)
            {
                appUser.PhoneNumber = editProfileRequest.PhoneNumber;
                appUser.PhoneNumberConfirmed = false;
            }

            bool updateResult = _userRepository.Update(appUser);
            if(!updateResult)
            {
                _logger.LogError($"Faild to update user. UserId {id}");
                return Result.Fail("error", "Error");
            }

            return Result.Ok();
        }

        public Result UnlockUser(UnlockUserRequest request, string adminId)
        {
            ValidationResult validationResult = _unlockUserValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invlid UnlockUserRequest. AdminId {adminId}");
                return Result.Fail(validationResult.Errors);
            }

            BaseSpecification<AppUserEntity> userSpecification = new BaseSpecification<AppUserEntity>();
            userSpecification.AddFilter(x => x.Id == request.UserId);

            AppUserEntity appUser = _userRepository.Get(userSpecification);
            if (appUser == null)
            {
                _logger.LogWarning($"No User. UserId {request.UserId}, AdminId {adminId}");
                return Result.Fail("no_user", "No user");
            }

            appUser.AccessFailedCount = 0;
            appUser.LockoutEnd = null;

            bool result = _userRepository.Update(appUser);
            if(!result)
            {
                _logger.LogError($"Faild to unlock user. AdminId {adminId}");
                return Result.Fail("error", "Error");
            }

            return Result.Ok();
        }

        public async Task<Result> SendEmilVerificationMail(SendEmailVerificationMailRequest request, string adminId)
        {
            ValidationResult validationResult = _sendEmailVerificationMailValidator.Validate(request);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invlid SendEmailVerificationMailRequest. AdminId {adminId}");
                return Result.Fail(validationResult.Errors);
            }

            BaseSpecification<AppUserEntity> userSpecification = new BaseSpecification<AppUserEntity>();
            userSpecification.AddFilter(x => x.Id == request.UserId);

            AppUserEntity appUser = _userRepository.Get(userSpecification);
            if (appUser == null)
            {
                _logger.LogWarning($"No User. UserId {request.UserId}, AdminId {adminId}");
                return Result.Fail("no_user", "No user");
            }

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

            await _emailConfirmationService.SendVerificationMail(appUser, code);

            return Result.Ok();
        }
    }
}
