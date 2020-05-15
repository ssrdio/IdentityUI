using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Group.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Group
{
    internal class GroupAttributeService : IGroupAttributeService
    {
        private readonly IBaseRepository<GroupEntity> _groupRepository;
        private readonly IBaseRepository<GroupAttributeEntity> _groupAttributeRepository;

        private readonly IValidator<AddGroupAttributeRequest> _addGroupAttributeValidator;
        private readonly IValidator<EditGroupAttributeRequest> _editGroupAttributeValidator;

        private readonly ILogger<GroupAttributeService> _logger;

        public GroupAttributeService(IBaseRepository<GroupEntity> groupRepository, IBaseRepository<GroupAttributeEntity> groupAttributeRepository,
            IValidator<AddGroupAttributeRequest> addGroupAttributeValidator, IValidator<EditGroupAttributeRequest> editGroupAttributeValidator,
            ILogger<GroupAttributeService> logger)
        {
            _groupRepository = groupRepository;
            _groupAttributeRepository = groupAttributeRepository;

            _addGroupAttributeValidator = addGroupAttributeValidator;
            _editGroupAttributeValidator = editGroupAttributeValidator;

            _logger = logger;
        }

        public Result Add(string groupId, AddGroupAttributeRequest addGroupAttribute)
        {
            ValidationResult validationResult = _addGroupAttributeValidator.Validate(addGroupAttribute);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(addGroupAttribute)} modal");
                return Result.Fail(validationResult.Errors);
            }

            BaseSpecification<GroupEntity> groupExistSpecification = new BaseSpecification<GroupEntity>();
            groupExistSpecification.AddFilter(x => x.Id == groupId);

            bool groupExist = _groupRepository.Exist(groupExistSpecification);
            if(!groupExist)
            {
                _logger.LogError($"No Group. GroupId {groupId}");
                return Result.Fail("no_group", "No Group");
            }

            BaseSpecification<GroupAttributeEntity> keyExistSpecification = new BaseSpecification<GroupAttributeEntity>();
            keyExistSpecification.AddFilter(x => x.GroupId == groupId);
            keyExistSpecification.AddFilter(x => x.Key.ToUpper() == addGroupAttribute.Key.ToUpper());

            bool keyExist = _groupAttributeRepository.Exist(keyExistSpecification);
            if(keyExist)
            {
                _logger.LogError($"GroupAttribute key already exist. GroupId {groupId}, key {addGroupAttribute.Key}");
                return Result.Fail("group_attribute_key_already_exist", "GroupAttribute key already exist");
            }

            GroupAttributeEntity groupAttributeEntity = new GroupAttributeEntity(
                key: addGroupAttribute.Key,
                value: addGroupAttribute.Value,
                groupId: groupId);

            bool addResult = _groupAttributeRepository.Add(groupAttributeEntity);
            if(!addResult)
            {
                _logger.LogError($"Failed to add GroupAttribute. GroupId {groupId}, Key {addGroupAttribute.Key}");
                return Result.Fail("failed_to_add_group_attribute", "Failed to add group attribute");
            }

            return Result.Ok();
        }

        private Result<GroupAttributeEntity> Get(string groupId, long id)
        {
            BaseSpecification<GroupAttributeEntity> baseSpecification = new BaseSpecification<GroupAttributeEntity>();
            baseSpecification.AddFilter(x => x.GroupId == groupId);
            baseSpecification.AddFilter(x => x.Id == id);

            GroupAttributeEntity groupAttribute = _groupAttributeRepository.SingleOrDefault(baseSpecification);
            if(groupAttribute == null)
            {
                _logger.LogError($"No GroupAttribute. GroupId {groupId}, Id {id}");
                return Result.Fail<GroupAttributeEntity>("no_group_attribute", "No GroupAttribute");
            }

            return Result.Ok(groupAttribute);
        }

        public Result Edit(string groupId, long id, EditGroupAttributeRequest editGroupAttribute)
        {
            ValidationResult validationResult = _editGroupAttributeValidator.Validate(editGroupAttribute);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(editGroupAttribute)} model");
                return Result.Fail(validationResult.Errors);
            }

            Result<GroupAttributeEntity> groupAttributeResult = Get(groupId, id);
            if(groupAttributeResult.Failure)
            {
                return Result.Fail(groupAttributeResult.Errors);
            }

            GroupAttributeEntity groupAttribute = groupAttributeResult.Value;

            groupAttribute.Update(editGroupAttribute.Value);
            bool updateResult = _groupAttributeRepository.Update(groupAttribute);
            if(!updateResult)
            {
                _logger.LogError($"Failed to update GroupAttribute. GroupId {groupId}, Id {id}");
                return Result.Fail("failed_to_update_group_attribute", "Failed to update GroupAttribute");
            }

            return Result.Ok();
        }

        public Result Remove(string groupId, long id)
        {
            Result<GroupAttributeEntity> groupAttributeResult = Get(groupId, id);
            if(groupAttributeResult.Failure)
            {
                return Result.Fail(groupAttributeResult.Errors);
            }

            bool removeResult = _groupAttributeRepository.Remove(groupAttributeResult.Value);
            if(!removeResult)
            {
                _logger.LogError($"Failed to remove GroupAttribute");
                return Result.Fail("failed_to_remove_group_attribute", "Failed to remove GroupAttribute");
            }

            return Result.Ok();
        }
    }
}
