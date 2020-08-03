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
using System.Linq;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities;

namespace SSRD.IdentityUI.Core.Services.User
{
    internal class ManageUserService : IManageUserService
    {
        private readonly UserManager<AppUserEntity> _userManager;

        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IBaseRepository<UserRoleEntity> _userRoleRepository;
        private readonly IBaseRepository<GroupUserEntity> _groupUserRepository;
        private readonly IBaseRepository<UserImageEntity> _userImageRepository;

        private readonly IEmailConfirmationService _emailConfirmationService;
        private readonly ISessionService _sessionService;

        private readonly IValidator<EditUserRequest> _editUserValidator;
        private readonly IValidator<SetNewPasswordRequest> _setNewPasswordValidator;
        private readonly IValidator<EditProfileRequest> _editProfileValidator;
        private readonly IValidator<UnlockUserRequest> _unlockUserValidator;
        private readonly IValidator<SendEmailVerificationMailRequest> _sendEmailVerificationMailValidator;

        private readonly ILogger<ManageUserService> _logger;

        public ManageUserService(UserManager<AppUserEntity> userManager, IUserRepository userRepository, IRoleRepository roleRepository,
            IBaseRepository<UserRoleEntity> userRoleRepository, IBaseRepository<GroupUserEntity> groupUserRepository,
            IBaseRepository<UserImageEntity> userImageRepository,
            IEmailConfirmationService emailConfirmationService, ISessionService sessionService,
            IValidator<EditUserRequest> editUserValidator, IValidator<SetNewPasswordRequest> setNewPasswordValidator,
            IValidator<EditProfileRequest> editRequestValidator, IValidator<UnlockUserRequest> unlockUserValidator,
            IValidator<SendEmailVerificationMailRequest> sendEmailVerificationMailValidator, ILogger<ManageUserService> logger)
        {
            _userManager = userManager;

            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _groupUserRepository = groupUserRepository;
            _userImageRepository = userImageRepository;

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
            if (!validationResult.IsValid)
            {
                _logger.LogError($"Invalid EditUserRequest. Admin {adminId}");
                return Result.Fail(ResultUtils.ToResultError(validationResult.Errors));
            }

            BaseSpecification<AppUserEntity> userSpecification = new BaseSpecification<AppUserEntity>();
            userSpecification.AddFilter(x => x.Id == id);

            AppUserEntity appUser = _userRepository.SingleOrDefault(userSpecification);
            if (appUser == null)
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

            if (appUser.TwoFactorEnabled)
            {
                appUser.TwoFactorEnabled = editUserRequest.TwoFactorEnabled;
            }

            if (appUser.Enabled != editUserRequest.Enabled)
            {
                appUser.Enabled = editUserRequest.Enabled;
                updateSecurityStamp = true;
            }

            bool result = _userRepository.Update(appUser);
            if (!result)
            {
                _logger.LogError($"Failed to save edited user data. Admin {adminId}");
                return Result.Fail("error", "error");
            }

            if (updateSecurityStamp)
            {
                Result logoutUserResult = await _sessionService.LogoutUser(new Auth.Session.Models.LogoutUserSessionsRequest(appUser.Id), adminId);
                if (logoutUserResult.Failure)
                {
                    return logoutUserResult;
                }
            }

            return Result.Ok();
        }

        public async Task<Result> SetNewPassword(string userId, SetNewPasswordRequest setNewPasswordRequest, string adminId)
        {
            ValidationResult validationResult = _setNewPasswordValidator.Validate(setNewPasswordRequest);
            if (!validationResult.IsValid)
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

            roles = roles.Select(x => x.ToUpper()).ToList();

            SelectSpecification<RoleEntity, string> roleSpecification = new SelectSpecification<RoleEntity, string>();
            roleSpecification.AddFilter(x => roles.Contains(x.NormalizedName));
            roleSpecification.AddFilter(x => x.Type == Data.Enums.Entity.RoleTypes.Global);
            roleSpecification.AddSelect(x => x.NormalizedName);

            List<string> existingRoles = _roleRepository.GetList(roleSpecification);
            if (roles.Count != existingRoles.Count)
            {
                _logger.LogError($"Some roles does not exists. Missing roles {Newtonsoft.Json.JsonConvert.SerializeObject(roles.Except(existingRoles))}");
            }

            SelectSpecification<UserRoleEntity, UserRoleEntity> getUserRolesSpecification = new SelectSpecification<UserRoleEntity, UserRoleEntity>();
            getUserRolesSpecification.AddFilter(x => x.UserId == userId);
            getUserRolesSpecification.AddFilter(x => x.Role.Type == Data.Enums.Entity.RoleTypes.Global);
            getUserRolesSpecification.AddFilter(x => existingRoles.Contains(x.Role.NormalizedName));
            getUserRolesSpecification.AddSelect(x => x);

            List<UserRoleEntity> userRoles = _userRoleRepository.GetList(getUserRolesSpecification);

            bool removeResult = _userRoleRepository.RemoveRange(userRoles);
            if (!removeResult)
            {
                _logger.LogError($"Failed to remove user roles. UserId {userId}. RoleNames {Newtonsoft.Json.JsonConvert.SerializeObject(roles)}");
                return Result.Fail("failed_to_remove_user_roles", "Failed to remove user roles");
            }

            _logger.LogInformation($"Removed roles form user {userId}. Role names: {Newtonsoft.Json.JsonConvert.SerializeObject(existingRoles)}");
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

            roles = roles.Select(x => x.ToUpper()).ToList();

            SelectSpecification<RoleEntity, RoleEntity> getRoleSpecification = new SelectSpecification<RoleEntity, RoleEntity>();
            getRoleSpecification.AddFilter(x => roles.Contains(x.NormalizedName));
            getRoleSpecification.AddSelect(x => x);

            List<RoleEntity> roleEntites = _roleRepository.GetList(getRoleSpecification);
            List<UserRoleEntity> userRoles = new List<UserRoleEntity>();

            foreach (RoleEntity role in roleEntites)
            {
                if (role.Type != Data.Enums.Entity.RoleTypes.Global)
                {
                    _logger.LogError($"Invalid role type. RoleId {role.Id}");
                    continue;
                }

                userRoles.Add(new UserRoleEntity(
                    userId: appUser.Id,
                    roleId: role.Id));
            }

            bool addRoles = _userRoleRepository.AddRange(userRoles);
            if (!addRoles)
            {
                _logger.LogError($"Failed to add user roles");
                return Result.Fail("failed_to_add_user_roles", "Failed to add UserRoles");
            }

            _logger.LogInformation($"Added roles to user. UserId {userId}, Role ids: {Newtonsoft.Json.JsonConvert.SerializeObject(userRoles.Select(x => x.RoleId))}");
            return Result.Ok();
        }

        public Result EditUser(string id, EditProfileRequest editProfileRequest)
        {
            ValidationResult validationResult = _editProfileValidator.Validate(editProfileRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invlid EditProfileRequest. UserId {id}");
                return Result.Fail(validationResult.Errors);
            }

            BaseSpecification<AppUserEntity> userSpecification = new BaseSpecification<AppUserEntity>();
            userSpecification.AddFilter(x => x.Id == id);

            AppUserEntity appUser = _userRepository.SingleOrDefault(userSpecification);
            if (appUser == null)
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
            if (!updateResult)
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

            AppUserEntity appUser = _userRepository.SingleOrDefault(userSpecification);
            if (appUser == null)
            {
                _logger.LogWarning($"No User. UserId {request.UserId}, AdminId {adminId}");
                return Result.Fail("no_user", "No user");
            }

            appUser.AccessFailedCount = 0;
            appUser.LockoutEnd = null;

            bool result = _userRepository.Update(appUser);
            if (!result)
            {
                _logger.LogError($"Faild to unlock user. AdminId {adminId}");
                return Result.Fail("error", "Error");
            }

            return Result.Ok();
        }

        public async Task<Result> SendEmilVerificationMail(SendEmailVerificationMailRequest request, string adminId)
        {
            ValidationResult validationResult = _sendEmailVerificationMailValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invlid SendEmailVerificationMailRequest. AdminId {adminId}");
                return Result.Fail(validationResult.Errors);
            }

            BaseSpecification<AppUserEntity> userSpecification = new BaseSpecification<AppUserEntity>();
            userSpecification.AddFilter(x => x.Id == request.UserId);

            AppUserEntity appUser = _userRepository.SingleOrDefault(userSpecification);
            if (appUser == null)
            {
                _logger.LogWarning($"No User. UserId {request.UserId}, AdminId {adminId}");
                return Result.Fail("no_user", "No user");
            }

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

            await _emailConfirmationService.SendVerificationMail(appUser, code);

            return Result.Ok();
        }

        public async Task<Result> RemoveRole(string userId, string roleId)
        {
            BaseSpecification<RoleEntity> getRoleSpecification = new BaseSpecification<RoleEntity>();
            getRoleSpecification.AddFilter(x => x.Id == roleId);

            RoleEntity role = _roleRepository.SingleOrDefault(getRoleSpecification);
            if (role == null)
            {
                _logger.LogError($"No role. RoleId {roleId}");
                return Result.Fail("no_role", "No Role");
            }

            switch (role.Type)
            {
                case Data.Enums.Entity.RoleTypes.Global:
                    {
                        return await RemoveGlobalRole(userId, role.Name);
                    }
                case Data.Enums.Entity.RoleTypes.Group:
                    {
                        return RemoveGroupRole(userId, roleId);
                    }
                default:
                    {
                        _logger.LogError($"Invalid Role type. RoleId {roleId}");
                        return Result.Fail("invalid_role_type", "Invalid role type");
                    }
            }
        }

        public Result UpdateProfileImage(string userId, byte[] image, string fileName)
        {
            if (image == null || image.Length < 1)
            {
                _logger.LogWarning($"Invlid Image. UserId {userId}");
                return Result.Fail("error", "Image is invalid, please select '.JPG' or '.PNG' image format.");
            }

            BaseSpecification<UserImageEntity> userImageSpecification = new BaseSpecification<UserImageEntity>();
            userImageSpecification.AddFilter(x => x.UserId == userId);

            if (_userImageRepository.Exist(userImageSpecification))
            {
                UserImageEntity userImage = _userImageRepository.SingleOrDefault(userImageSpecification);
                userImage.BlobImage = image;
                userImage.FileName = fileName;

                bool updateResult = _userImageRepository.Update(userImage);
                if (!updateResult)
                {
                    _logger.LogError($"Faild to update user image. UserId {userId}");
                    return Result.Fail("error", "Error");
                }
            }
            else
            {
                _userImageRepository.Add(new UserImageEntity
                {
                    BlobImage = image,
                    FileName = fileName,
                    UserId = userId,
                    IsDefault = true
                });
            }
            return Result.Ok();
        }

        public Result<string> GetProfileImageURL(string userId)
        {
            BaseSpecification<UserImageEntity> userImageSpecification = new BaseSpecification<UserImageEntity>();
            userImageSpecification.AddFilter(x => x.UserId == userId);

            if (_userImageRepository.Exist(userImageSpecification))
            {
                UserImageEntity userImage = _userImageRepository.SingleOrDefault(userImageSpecification);
                return Result.Ok(userImage.URL);
            }

            return Result.Ok(default(string));
        }

        private async Task<Result> RemoveGlobalRole(string userId, string roleName)
        {
            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                _logger.LogWarning($"No User. UserId {userId}.");
                return Result.Fail("no_user", "No User");
            }

            IdentityResult removeRoleResult = await _userManager.RemoveFromRoleAsync(appUser, roleName);
            if (!removeRoleResult.Succeeded)
            {
                _logger.LogError($"Failed to remove global role. UserId {userId}, RoleName {roleName}");
                return Result.Fail("failed_to_remove_role", "Failed to remove role");
            }

            return Result.Ok();
        }

        private Result RemoveGroupRole(string userId, string roleId)
        {
            BaseSpecification<GroupUserEntity> baseSpecification = new BaseSpecification<GroupUserEntity>();
            baseSpecification.AddFilter(x => x.UserId == userId);
            baseSpecification.AddFilter(x => x.RoleId == roleId);

            GroupUserEntity groupUser = _groupUserRepository.SingleOrDefault(baseSpecification);
            if (groupUser == null)
            {
                _logger.LogError($"No GroupUser. UserId {userId}, roleId {roleId}");
                return Result.Fail("no_group_user", "No GroupUser");
            }

            groupUser.UpdateRole(null);

            bool updateResult = _groupUserRepository.Update(groupUser);
            if (!updateResult)
            {
                _logger.LogError($"Failed to update GroupUser. UserId {userId}, roleId {roleId}");
                return Result.Fail("failed_to_update_group_user", "Failed to update GroupUser");
            }

            return Result.Ok();
        }
    }
}
