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

namespace SSRD.IdentityUI.Core.Services.User
{
    internal class AddUserService : IAddUserService
    {
        private readonly UserManager<AppUserEntity> _userManager;

        private readonly IEmailService _emailService;

        private readonly IValidator<NewUserRequest> _newUserValidator;
        private readonly IValidator<RegisterRequest> _registerValidator;

        private readonly ILogger<AddUserService> _logger;

        public AddUserService(UserManager<AppUserEntity> userManager, IValidator<NewUserRequest> newUserValidator,
            IValidator<RegisterRequest> registerValidator, ILogger<AddUserService> logger, IEmailService emailService)
        {
            _userManager = userManager;

            _newUserValidator = newUserValidator;
            _registerValidator = registerValidator;

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
                _logger.LogError($"Admin with id {adminId} faild to add new user");
                return Result.Fail<string>(ResultUtils.ToResultError(result.Errors));
            }

            appUser = await _userManager.FindByNameAsync(newUserRequest.UserName);
            if (appUser == null)
            {
                _logger.LogError($"Faild to find new user with UserName {newUserRequest.UserName}. Admin {adminId}");
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
                _logger.LogError($"Faild to register user");
                return Result.Fail(ResultUtils.ToResultError(identityResult.Errors));
            }

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

            await _emailService.SendVerificationMail(appUser, code);

            return Result.Ok();
        }
    }
}
