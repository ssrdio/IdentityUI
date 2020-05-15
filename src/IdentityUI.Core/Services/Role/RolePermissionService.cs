using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Group;
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
    internal class RolePermissionService : IRolePermissionService
    {
        private readonly IBaseRepository<RoleEntity> _roleRepository;
        private readonly IBaseRepository<PermissionEntity> _permissionRepository;
        private readonly IBaseRepository<PermissionRoleEntity> _permissionRoleRepository;

        private readonly IValidator<AddRolePermissionRequest> _addRolePermissionValidator;

        private readonly ILogger<RolePermissionService> _logger;

        public RolePermissionService(IBaseRepository<RoleEntity> roleRepository, IBaseRepository<PermissionEntity> permissionRepository,
            IBaseRepository<PermissionRoleEntity> permissionRoleRepository, IValidator<AddRolePermissionRequest> addRolePermissionValidator,
            ILogger<RolePermissionService> logger)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _permissionRoleRepository = permissionRoleRepository;

            _addRolePermissionValidator = addRolePermissionValidator;

            _logger = logger;
        }

        public Result Add(string roleId, AddRolePermissionRequest addRolePermission)
        {
            ValidationResult validationResult = _addRolePermissionValidator.Validate(addRolePermission);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(addRolePermission)} model");
                return Result.Fail(validationResult.Errors);
            }

            BaseSpecification<RoleEntity> roleExistSpecification = new BaseSpecification<RoleEntity>();
            roleExistSpecification.AddFilter(x => x.Id == roleId);

            bool roleExist = _roleRepository.Exist(roleExistSpecification);
            if (!roleExist)
            {
                _logger.LogError($"No Role. RoleId {roleId}");
                return Result.Fail("no_role", "No Role");
            }

            BaseSpecification<PermissionEntity> permissionExistSpecification = new BaseSpecification<PermissionEntity>();
            permissionExistSpecification.AddFilter(x => x.Id == addRolePermission.PermissionId);

            bool groupRoleExist = _permissionRepository.Exist(permissionExistSpecification);
            if (!groupRoleExist)
            {
                _logger.LogError($"No Permission. PermissionId {addRolePermission.PermissionId}");
                return Result.Fail("no_group_role", "No GroupRole");
            }

            BaseSpecification<PermissionRoleEntity> permissionRoleExistSpecification = new BaseSpecification<PermissionRoleEntity>();
            permissionRoleExistSpecification.AddFilter(x => x.RoleId == roleId);
            permissionRoleExistSpecification.AddFilter(x => x.PermissionId == addRolePermission.PermissionId);

            bool groupRoleIntermediateExist = _permissionRoleRepository.Exist(permissionRoleExistSpecification);
            if (groupRoleIntermediateExist)
            {
                _logger.LogError($"PermissionRole already exist.");
                return Result.Fail("permission_role_already_exist", "Permission Role already exist");
            }

            PermissionRoleEntity permissionRole = new PermissionRoleEntity(
                roleId: roleId,
                permissionId: addRolePermission.PermissionId);

            bool addResult = _permissionRoleRepository.Add(permissionRole);
            if (!addResult)
            {
                _logger.LogError($"Failed to add PermissionRole. RoleId {roleId}, PermissionId {addRolePermission.PermissionId}");
                return Result.Fail("failed_to_add_permission_role", "Failed to add Permission Role");
            }

            return Result.Ok();
        }

        private Result Remove(BaseSpecification<PermissionRoleEntity> baseSpecification)
        {
            PermissionRoleEntity permissionRole = _permissionRoleRepository.SingleOrDefault(baseSpecification);
            if (permissionRole == null)
            {
                _logger.LogError($"No PermissionRole.");
                return Result.Fail("no_permission_role", "No Permission Role");
            }

            bool removResult = _permissionRoleRepository.Remove(permissionRole);
            if (!removResult)
            {
                _logger.LogError($"Failed to remove PermissionRole.");
                return Result.Fail("failed_to_remove_permission_role", "Failed to remove Permission Role");
            }

            return Result.Ok();
        }

        public Result Remove(long permissionRoleId)
        {
            _logger.LogInformation($"Removing PermissionRole. PermissionRoleId {permissionRoleId}");

            BaseSpecification<PermissionRoleEntity> baseSpecification = new BaseSpecification<PermissionRoleEntity>();
            baseSpecification.AddFilter(x => x.Id == permissionRoleId);

            return Remove(baseSpecification);
        }

        public Result Remove(string roleId, string permissionId)
        {
            _logger.LogInformation($"Removing PermissionRole. RoleId {roleId}, PermissionId {permissionId}");

            BaseSpecification<PermissionRoleEntity> baseSpecification = new BaseSpecification<PermissionRoleEntity>();
            baseSpecification.AddFilter(x => x.RoleId == roleId);
            baseSpecification.AddFilter(x => x.PermissionId == permissionId);

            return Remove(baseSpecification);
        }
    }
}
