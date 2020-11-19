using FluentValidation;
using FluentValidation.Results;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Services.User.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Interfaces;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.CommonUtils.Result;

namespace SSRD.IdentityUI.Core.Services.User
{
    internal class ManageUserService : IManageUserService
    {
        private const string USER_NOT_FOUND = "user_not_found";
        private const string FAILED_TO_REMOVE_USER = "failed_to_remove_user";
        private const string FAILED_TO_UPDATE_USER = "failed_to_update_user";

        private readonly UserManager<AppUserEntity> _userManager;

        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IBaseRepository<UserRoleEntity> _userRoleRepository;
        private readonly IBaseRepository<GroupUserEntity> _groupUserRepository;

        private readonly IBaseDAO<AppUserEntity> _userDAO;

        private readonly IGroupUserStore _groupUserStore;

        private readonly IEmailConfirmationService _emailConfirmationService;
        private readonly ISessionService _sessionService;
        private readonly IProfileImageService _profileImageService;

        private readonly IValidator<EditUserRequest> _editUserValidator;
        private readonly IValidator<SetNewPasswordRequest> _setNewPasswordValidator;
        private readonly IValidator<EditProfileRequest> _editProfileValidator;
        private readonly IValidator<UnlockUserRequest> _unlockUserValidator;
        private readonly IValidator<SendEmailVerificationMailRequest> _sendEmailVerificationMailValidator;

        private readonly ILogger<ManageUserService> _logger;

        public ManageUserService(
            UserManager<AppUserEntity> userManager,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IBaseRepository<UserRoleEntity> userRoleRepository,
            IBaseRepository<GroupUserEntity> groupUserRepository,
            IBaseDAO<AppUserEntity> userDAO,
            IGroupUserStore groupUserStore,
            IEmailConfirmationService emailConfirmationService,
            ISessionService sessionService,
            IProfileImageService profileImageService,
            IValidator<EditUserRequest> editUserValidator,
            IValidator<SetNewPasswordRequest> setNewPasswordValidator,
            IValidator<EditProfileRequest> editRequestValidator,
            IValidator<UnlockUserRequest> unlockUserValidator,
            IValidator<SendEmailVerificationMailRequest> sendEmailVerificationMailValidator,
            ILogger<ManageUserService> logger)
        {
            _userManager = userManager;

            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _groupUserRepository = groupUserRepository;

            _userDAO = userDAO;

            _groupUserStore = groupUserStore;

            _emailConfirmationService = emailConfirmationService;
            _sessionService = sessionService;

            _profileImageService = profileImageService;

            _editUserValidator = editUserValidator;
            _setNewPasswordValidator = setNewPasswordValidator;
            _editProfileValidator = editRequestValidator;

            _unlockUserValidator = unlockUserValidator;
            _sendEmailVerificationMailValidator = sendEmailVerificationMailValidator;

            _logger = logger;
        }

        public async Task<Core.Models.Result.Result> EditUser(string id, EditUserRequest editUserRequest, string adminId)
        {
            ValidationResult validationResult = _editUserValidator.Validate(editUserRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogError($"Invalid EditUserRequest. Admin {adminId}");
                return Core.Models.Result.Result.Fail(ResultUtils.ToResultError(validationResult.Errors));
            }

            BaseSpecification<AppUserEntity> userSpecification = new BaseSpecification<AppUserEntity>();
            userSpecification.AddFilter(x => x.Id == id);

            AppUserEntity appUser = _userRepository.SingleOrDefault(userSpecification);
            if (appUser == null)
            {
                _logger.LogWarning($"No User. UserId {id}. Admin {adminId}");
                return Core.Models.Result.Result.Fail("no_user", "No User");
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
                return Core.Models.Result.Result.Fail("error", "error");
            }

            if (updateSecurityStamp)
            {
                Core.Models.Result.Result logoutUserResult = await _sessionService.LogoutUser(new Auth.Session.Models.LogoutUserSessionsRequest(appUser.Id), adminId);
                if (logoutUserResult.Failure)
                {
                    return logoutUserResult;
                }
            }

            return Core.Models.Result.Result.Ok();
        }

        public async Task<Core.Models.Result.Result> SetNewPassword(string userId, SetNewPasswordRequest setNewPasswordRequest, string adminId)
        {
            ValidationResult validationResult = _setNewPasswordValidator.Validate(setNewPasswordRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogError($"Invlid SetNewPasswordRequest. Admin {adminId}");
                return Core.Models.Result.Result.Fail(ResultUtils.ToResultError(validationResult.Errors));
            }

            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                _logger.LogError($"No User with id {userId}. Admin {adminId}");
                return Core.Models.Result.Result.Fail("no_user", "No User");
            }

            _logger.LogInformation($"Seting new password for with id {userId}. Admin id {adminId}");

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(appUser);

            IdentityResult changePasswordResult = await _userManager.ResetPasswordAsync(appUser, passwordResetToken, setNewPasswordRequest.Password);
            if (!changePasswordResult.Succeeded)
            {
                _logger.LogError($"Faild to reset password. UserId {appUser.Id}, admin {adminId}");
                return Core.Models.Result.Result.Fail(changePasswordResult.Errors);
            }


            Core.Models.Result.Result logoutUserResult = await _sessionService.LogoutUser(new Auth.Session.Models.LogoutUserSessionsRequest(appUser.Id), adminId);
            if (logoutUserResult.Failure)
            {
                return logoutUserResult;
            }

            _logger.LogInformation($"Added new password to user with id {userId}. Admin id {adminId}");
            return Core.Models.Result.Result.Ok();
        }

        public async Task<Core.Models.Result.Result> RemoveRoles(string userId, List<string> roles, string adminId)
        {
            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                _logger.LogError($"No user {userId}");
                return Core.Models.Result.Result.Fail("no_user", "No User");
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
                return Core.Models.Result.Result.Fail("failed_to_remove_user_roles", "Failed to remove user roles");
            }

            _logger.LogInformation($"Removed roles form user {userId}. Role names: {Newtonsoft.Json.JsonConvert.SerializeObject(existingRoles)}");
            return Core.Models.Result.Result.Ok();
        }

        public async Task<Core.Models.Result.Result> AddRoles(string userId, List<string> roles, string adminId)
        {
            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                _logger.LogError($"No user {userId}");
                return Core.Models.Result.Result.Fail("no_user", "No User");
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
                return Core.Models.Result.Result.Fail("failed_to_add_user_roles", "Failed to add UserRoles");
            }

            _logger.LogInformation($"Added roles to user. UserId {userId}, Role ids: {Newtonsoft.Json.JsonConvert.SerializeObject(userRoles.Select(x => x.RoleId))}");
            return Core.Models.Result.Result.Ok();
        }

        public Core.Models.Result.Result EditUser(string id, EditProfileRequest editProfileRequest)
        {
            ValidationResult validationResult = _editProfileValidator.Validate(editProfileRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invlid EditProfileRequest. UserId {id}");
                return Core.Models.Result.Result.Fail(validationResult.Errors);
            }

            BaseSpecification<AppUserEntity> userSpecification = new BaseSpecification<AppUserEntity>();
            userSpecification.AddFilter(x => x.Id == id);

            AppUserEntity appUser = _userRepository.SingleOrDefault(userSpecification);
            if (appUser == null)
            {
                _logger.LogWarning($"No User. UserId {id}");
                return Core.Models.Result.Result.Fail("no_user", "No user");
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
                return Core.Models.Result.Result.Fail("error", "Error");
            }

            return Core.Models.Result.Result.Ok();
        }

        public Core.Models.Result.Result UnlockUser(UnlockUserRequest request, string adminId)
        {
            ValidationResult validationResult = _unlockUserValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invlid UnlockUserRequest. AdminId {adminId}");
                return Core.Models.Result.Result.Fail(validationResult.Errors);
            }

            BaseSpecification<AppUserEntity> userSpecification = new BaseSpecification<AppUserEntity>();
            userSpecification.AddFilter(x => x.Id == request.UserId);

            AppUserEntity appUser = _userRepository.SingleOrDefault(userSpecification);
            if (appUser == null)
            {
                _logger.LogWarning($"No User. UserId {request.UserId}, AdminId {adminId}");
                return Core.Models.Result.Result.Fail("no_user", "No user");
            }

            appUser.AccessFailedCount = 0;
            appUser.LockoutEnd = null;

            bool result = _userRepository.Update(appUser);
            if (!result)
            {
                _logger.LogError($"Faild to unlock user. AdminId {adminId}");
                return Core.Models.Result.Result.Fail("error", "Error");
            }

            return Core.Models.Result.Result.Ok();
        }

        public async Task<Core.Models.Result.Result> SendEmilVerificationMail(SendEmailVerificationMailRequest request, string adminId)
        {
            ValidationResult validationResult = _sendEmailVerificationMailValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invlid SendEmailVerificationMailRequest. AdminId {adminId}");
                return Core.Models.Result.Result.Fail(validationResult.Errors);
            }

            BaseSpecification<AppUserEntity> userSpecification = new BaseSpecification<AppUserEntity>();
            userSpecification.AddFilter(x => x.Id == request.UserId);

            AppUserEntity appUser = _userRepository.SingleOrDefault(userSpecification);
            if (appUser == null)
            {
                _logger.LogWarning($"No User. UserId {request.UserId}, AdminId {adminId}");
                return Core.Models.Result.Result.Fail("no_user", "No user");
            }

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

            await _emailConfirmationService.SendVerificationMail(appUser, code);

            return Core.Models.Result.Result.Ok();
        }

        public async Task<Core.Models.Result.Result> RemoveRole(string userId, string roleId)
        {
            BaseSpecification<RoleEntity> getRoleSpecification = new BaseSpecification<RoleEntity>();
            getRoleSpecification.AddFilter(x => x.Id == roleId);

            RoleEntity role = _roleRepository.SingleOrDefault(getRoleSpecification);
            if (role == null)
            {
                _logger.LogError($"No role. RoleId {roleId}");
                return Core.Models.Result.Result.Fail("no_role", "No Role");
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
                        return Core.Models.Result.Result.Fail("invalid_role_type", "Invalid role type");
                    }
            }
        }

        private async Task<Core.Models.Result.Result> RemoveGlobalRole(string userId, string roleName)
        {
            AppUserEntity appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                _logger.LogWarning($"No User. UserId {userId}.");
                return Core.Models.Result.Result.Fail("no_user", "No User");
            }

            IdentityResult removeRoleResult = await _userManager.RemoveFromRoleAsync(appUser, roleName);
            if (!removeRoleResult.Succeeded)
            {
                _logger.LogError($"Failed to remove global role. UserId {userId}, RoleName {roleName}");
                return Core.Models.Result.Result.Fail("failed_to_remove_role", "Failed to remove role");
            }

            return Core.Models.Result.Result.Ok();
        }

        private Core.Models.Result.Result RemoveGroupRole(string userId, string roleId)
        {
            BaseSpecification<GroupUserEntity> baseSpecification = new BaseSpecification<GroupUserEntity>();
            baseSpecification.AddFilter(x => x.UserId == userId);
            baseSpecification.AddFilter(x => x.RoleId == roleId);

            GroupUserEntity groupUser = _groupUserRepository.SingleOrDefault(baseSpecification);
            if (groupUser == null)
            {
                _logger.LogError($"No GroupUser. UserId {userId}, roleId {roleId}");
                return Core.Models.Result.Result.Fail("no_group_user", "No GroupUser");
            }

            groupUser.UpdateRole(null);

            bool updateResult = _groupUserRepository.Update(groupUser);
            if (!updateResult)
            {
                _logger.LogError($"Failed to update GroupUser. UserId {userId}, roleId {roleId}");
                return Core.Models.Result.Result.Fail("failed_to_update_group_user", "Failed to update GroupUser");
            }

            return Core.Models.Result.Result.Ok();
        }

        public async Task<CommonUtils.Result.Result> EditUser(long groupUserId, EditUserRequest editUserRequest)
        {
            IBaseSpecification<GroupUserEntity, GroupUserEntity> specification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => x.Id == groupUserId)
                .Include(x => x.User)
                .Build();

            CommonUtils.Result.Result<GroupUserEntity> getGroupUserResult = await _groupUserStore.SingleOrDefault(specification);
            if(getGroupUserResult.Failure)
            {
                return CommonUtils.Result.Result.Fail(getGroupUserResult);
            }

            //TODO: change so that Edit takes AppUserEntity as parameter
            Core.Models.Result.Result updateResult = await EditUser(getGroupUserResult.Value.UserId, editUserRequest, "");

            return updateResult.ToNewResult();
        }

        public async Task<CommonUtils.Result.Result> UnlockUser(long groupUserId)
        {
            IBaseSpecification<GroupUserEntity, GroupUserEntity> specification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => x.Id == groupUserId)
                .Include(x => x.User)
                .Build();

            CommonUtils.Result.Result<GroupUserEntity> getGroupUserResult = await _groupUserStore.SingleOrDefault(specification);
            if (getGroupUserResult.Failure)
            {
                return CommonUtils.Result.Result.Fail(getGroupUserResult);
            }

            UnlockUserRequest unlockUserRequest = new UnlockUserRequest(
                userId: getGroupUserResult.Value.UserId);

            Core.Models.Result.Result resuilt = UnlockUser(unlockUserRequest, "");

            return resuilt.ToNewResult();
        }

        public async Task<CommonUtils.Result.Result> SendEmilVerificationMail(long groupUserId)
        {
            IBaseSpecification<GroupUserEntity, GroupUserEntity> specification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => x.Id == groupUserId)
                .Include(x => x.User)
                .Build();

            CommonUtils.Result.Result<GroupUserEntity> getGroupUserResult = await _groupUserStore.SingleOrDefault(specification);
            if (getGroupUserResult.Failure)
            {
                return CommonUtils.Result.Result.Fail(getGroupUserResult);
            }

            SendEmailVerificationMailRequest sendEmailVerificationMailRequest = new SendEmailVerificationMailRequest(
                userId: getGroupUserResult.Value.UserId);

            Core.Models.Result.Result result = await SendEmilVerificationMail(sendEmailVerificationMailRequest, "");

            return result.ToNewResult();
        }

        public async Task<Result> RemoveUser(string userId)
        {
            _logger.LogInformation($"Removing user. UserId {userId}");

            IBaseSpecification<AppUserEntity, AppUserEntity> getUserSpecification = SpecificationBuilder
                .Create<AppUserEntity>()
                .Where(x => x.Id == userId)
                .Build();

            AppUserEntity appUser = await _userDAO.SingleOrDefault(getUserSpecification);
            if (appUser == null)
            {
                _logger.LogWarning($"No User. UserId {userId}");
                return Result.Fail(USER_NOT_FOUND);
            }

            Result removeUserImageResult = await _profileImageService.Remove(userId);
            if (removeUserImageResult.Failure)
            {
                return Result.Fail(removeUserImageResult);
            }

            appUser.FirstName = null;
            appUser.LastName = null;

            Guid guid = Guid.NewGuid();

            appUser.Email = $"deleted_email_{guid}";
            appUser.UserName = $"deleted_username_{guid}";
            appUser.SecurityStamp = Guid.NewGuid().ToString();

#if NET_CORE2
            appUser.NormalizedEmail = _userManager.NormalizeKey(appUser.Email);
            appUser.NormalizedEmail = _userManager.NormalizeKey(appUser.UserName);
#elif NET_CORE3
            appUser.NormalizedEmail = _userManager.NormalizeEmail(appUser.Email);
            appUser.NormalizedUserName = _userManager.NormalizeName(appUser.UserName);
#endif

            bool updateResult = await _userDAO.Update(appUser);
            if (!updateResult)
            {
                _logger.LogError($"Failed to update user. UserId {userId}");
                return Result.Fail(FAILED_TO_UPDATE_USER);
            }

            bool removeResult = _userRepository.Remove(appUser);
            if (!removeResult)
            {
                _logger.LogError($"Failed to remove user. UserId {userId}");
                return Result.Fail(FAILED_TO_REMOVE_USER);
            }

            _logger.LogInformation($"User was removed. UserId {userId}");

            return Result.Ok();
        }

    }
}