using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services.Role;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Role.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Role
{
    internal class RoleAssignmentService : IRoleAssignmentService
    {
        private readonly IBaseRepository<RoleEntity> _roleRepository;
        private readonly IBaseRepository<RoleAssignmentEntity> _groupRoleAssignmentRepository;

        private readonly IValidator<AddRoleAssignmentRequest> _addRoleAssignmentValidator;

        private readonly ILogger<RoleAssignmentService> _logger;

        public RoleAssignmentService(IBaseRepository<RoleEntity> roleRepository, IBaseRepository<RoleAssignmentEntity> grouproleAssignmetRepository,
            IValidator<AddRoleAssignmentRequest> addRoleAssignmentValidator, ILogger<RoleAssignmentService> logger)
        {
            _roleRepository = roleRepository;
            _groupRoleAssignmentRepository = grouproleAssignmetRepository;

            _addRoleAssignmentValidator = addRoleAssignmentValidator;

            _logger = logger;
        }

        private Result ValidateRole(string roleId)
        {
            BaseSpecification<RoleEntity> baseSpecification = new BaseSpecification<RoleEntity>();
            baseSpecification.AddFilter(x => x.Id == roleId);

            RoleEntity role = _roleRepository.SingleOrDefault(baseSpecification);
            if (role == null)
            {
                _logger.LogError($"No Role. RoleId {roleId}");
                return Result.Fail("no_role", "No Role");
            }

            if(role.Type != Data.Enums.Entity.RoleTypes.Group)
            {
                _logger.LogError($"Invalid Role type. RoleId {roleId}");
                return Result.Fail("invalid_role_type", "Invalid role type");
            }

            return Result.Ok();
        }

        public Result Add(string roleId, AddRoleAssignmentRequest addRoleAssignment)
        {
            ValidationResult validationResult = _addRoleAssignmentValidator.Validate(addRoleAssignment);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid AddRoleAssignmentRequest model");
                return Result.Fail(validationResult.Errors);
            }

            if (roleId == addRoleAssignment.RoleId)
            {
                _logger.LogError($"Can not assign Role to itself. RoleId {roleId}");
                return Result.Fail("can_not_assigne_to_itself", "Can not assign Role to itself");
            }

            Result roleExist = ValidateRole(roleId);
            if (roleExist.Failure)
            {
                return Result.Fail(roleExist.Errors);
            }

            Result canAssigneRoleExist = ValidateRole(addRoleAssignment.RoleId);
            if (canAssigneRoleExist.Failure)
            {
                return Result.Fail(canAssigneRoleExist.Errors);
            }

            BaseSpecification<RoleAssignmentEntity> roleAlreadyAssignedSpecification = new BaseSpecification<RoleAssignmentEntity>();
            roleAlreadyAssignedSpecification.AddFilter(x => x.RoleId == roleId);
            roleAlreadyAssignedSpecification.AddFilter(x => x.CanAssigneRoleId == addRoleAssignment.RoleId);

            bool roleAlreadyAssigned = _groupRoleAssignmentRepository.Exist(roleAlreadyAssignedSpecification);
            if (roleAlreadyAssigned)
            {
                _logger.LogError($"Role is already assigned. RoleId {roleId}, CanAssigneRoleId {addRoleAssignment.RoleId}");
                return Result.Fail("role_is_already_assigned", "Role is already assigned");
            }

            //TODO: check for recursion loop if we are going to use role assignment recursion

            RoleAssignmentEntity roleAssignment = new RoleAssignmentEntity(
                roleId: roleId,
                canAssigneRoleId: addRoleAssignment.RoleId);

            bool addResult = _groupRoleAssignmentRepository.Add(roleAssignment);
            if (!addResult)
            {
                _logger.LogError($"Failed to add GroupRoleAssignment, RoleId {roleId}, CanAssignedRoleId {addRoleAssignment.RoleId}");
                return Result.Fail("failed_to_add_role_assignment", "Failed to add RoleAssignment");
            }

            return Result.Ok();
        }

        private Result Remove(BaseSpecification<RoleAssignmentEntity> baseSpecification)
        {
            RoleAssignmentEntity roleAssignment = _groupRoleAssignmentRepository.SingleOrDefault(baseSpecification);
            if (roleAssignment == null)
            {
                _logger.LogError($"No RoleAssignment");
                return Result.Fail("no_role_assignment", "No RoleAssignment");
            }

            bool removeResult = _groupRoleAssignmentRepository.Remove(roleAssignment);
            if (!removeResult)
            {
                _logger.LogError($"Failed to remove RoleAssignemt");
                return Result.Fail("failed_to_remove_role_assignment", "Failed to remove RoleAssignment");
            }

            return Result.Ok();
        }

        public Result Remove(string roleId, string assigneRoleId)
        {
            _logger.LogInformation($"Removing RoleAssignment. RoleId {roleId}, AssignedRoleId {assigneRoleId}");

            BaseSpecification<RoleAssignmentEntity> baseSpecification = new BaseSpecification<RoleAssignmentEntity>();
            baseSpecification.AddFilter(x => x.RoleId == roleId);
            baseSpecification.AddFilter(x => x.CanAssigneRoleId == assigneRoleId);

            return Remove(baseSpecification);
        }

        public Result Remove(long id)
        {
            _logger.LogInformation($"Removing RoleAssignment. RoleAssignmentId {id}");

            BaseSpecification<RoleAssignmentEntity> baseSpecification = new BaseSpecification<RoleAssignmentEntity>();
            baseSpecification.AddFilter(x => x.Id == id);

            return Remove(baseSpecification);
        }
    }
}
