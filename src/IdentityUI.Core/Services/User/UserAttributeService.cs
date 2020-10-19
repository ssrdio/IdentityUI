using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Entities.User;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.User.Models.Attribute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.User
{
    internal class UserAttributeService : IUserAttributeService
    {
        private readonly IBaseRepositoryAsync<UserAttributeEntity> _userAttributeRepository;
        private readonly IBaseRepositoryAsync<AppUserEntity> _userRepository;

        private readonly IValidator<AddUserAttributeModel> _addUserAttributeValidator;
        private readonly IValidator<UpdateUserAttributeModel> _updateUserAttribute;

        private readonly ILogger<UserAttributeService> _logger;

        public UserAttributeService(
            IBaseRepositoryAsync<UserAttributeEntity> userAttributeRepository,
            IBaseRepositoryAsync<AppUserEntity> userRepository,
            IValidator<AddUserAttributeModel> addUserAttributeValidator,
            IValidator<UpdateUserAttributeModel> updateUserAttribute,
            ILogger<UserAttributeService> logger)
        {
            _userAttributeRepository = userAttributeRepository;
            _userRepository = userRepository;
            _addUserAttributeValidator = addUserAttributeValidator;
            _updateUserAttribute = updateUserAttribute;
            _logger = logger;
        }

        public async Task<Result> Add(string userId, AddUserAttributeModel addUserAttribute)
        {
            ValidationResult validationResult = _addUserAttributeValidator.Validate(addUserAttribute);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {typeof(AddUserAttributeModel).Name} model");
                return Result.Fail(validationResult.Errors);
            }

            BaseSpecification<AppUserEntity> userSpecification = new BaseSpecification<AppUserEntity>();
            userSpecification.AddFilter(x => x.Id == userId);

            bool userExist = await _userRepository.Exist(userSpecification);
            if(!userExist)
            {
                _logger.LogError($"No User. UserId {userId}");
                return Result.Fail("no_user", "No User");
            }

            BaseSpecification<UserAttributeEntity> keyAlreadyExistsSpecification = new BaseSpecification<UserAttributeEntity>();
            keyAlreadyExistsSpecification.AddFilter(x => x.UserId == userId);
            keyAlreadyExistsSpecification.AddFilter(x => x.Key == addUserAttribute.Key);

            bool keyAlreadyExists = await _userAttributeRepository.Exist(keyAlreadyExistsSpecification);
            if(keyAlreadyExists)
            {
                _logger.LogError($"Key already exists. UserId {userId}, Key {addUserAttribute.Key}");
                return Result.Fail("key_already_exists", "Key already exists");
            }

            UserAttributeEntity userAttribute = new UserAttributeEntity(
                key: addUserAttribute.Key,
                value: addUserAttribute.Value,
                userId: userId);

            bool addAttributeResult = await _userAttributeRepository.Add(userAttribute);
            if(!addAttributeResult)
            {
                _logger.LogError($"Failed to add user attribute. UserId {userId}");
                return Result.Fail("failed_to_add_user_attribute", "Failed to add user attribute");
            }

            return Result.Ok(userAttribute);
        }

        public async Task<Result> Update(string userId, long attributeId, UpdateUserAttributeModel updateUserAttribute)
        {
            ValidationResult validationResult = _updateUserAttribute.Validate(updateUserAttribute);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {typeof(UpdateUserAttributeModel).CustomAttributes} model");
                return Result.Fail(validationResult.Errors);
            }

            BaseSpecification<UserAttributeEntity> baseSpecification = new BaseSpecification<UserAttributeEntity>();
            baseSpecification.AddFilter(x => x.Id == attributeId);
            baseSpecification.AddFilter(x => x.UserId == userId);

            UserAttributeEntity userAttribute = await _userAttributeRepository.SingleOrDefault(baseSpecification);
            if(userAttribute == null)
            {
                _logger.LogError($"No UserAttribute. UserId {userId}, AttributeId {attributeId}");
                return Result.Fail("no_user_attribute", "No UserAttribute");
            }

            userAttribute.Value = updateUserAttribute.Value;

            bool updateResult = await _userAttributeRepository.Update(userAttribute);
            if(!updateResult)
            {
                _logger.LogError($"Failed to update UserAttribute. UserId {userId}, AttributeId {attributeId}");
                return Result.Fail("failed_to_update_user_attribute", "Failed to update user attribute");
            }

            return Result.Ok();
        }

        public async Task<Result> Remove(string userId, long attributeId)
        {
            BaseSpecification<UserAttributeEntity> baseSpecification = new BaseSpecification<UserAttributeEntity>();
            baseSpecification.AddFilter(x => x.Id == attributeId);
            baseSpecification.AddFilter(x => x.UserId == userId);

            UserAttributeEntity userAttribute = await _userAttributeRepository.SingleOrDefault(baseSpecification);
            if (userAttribute == null)
            {
                _logger.LogError($"No UserAttribute. UserId {userId}, AttributeId {attributeId}");
                return Result.Fail("no_user_attribute", "No UserAttribute");
            }

            bool removeResult = await _userAttributeRepository.Remove(userAttribute);
            if(!removeResult)
            {
                _logger.LogError($"Failed to remove user attribute. UserId {userId}, AttributeId {attributeId}");
                return Result.Fail("failed_to_remove_user_attribute", "Failed to remove user attribute");
            }

            return Result.Ok();
        }
    }
}
