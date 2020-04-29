using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Permission.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Permission
{
    internal class PermissionService : IPermissionService
    {
        private readonly IBaseRepository<PermissionEntity> _permissionRepository;

        private readonly IValidator<AddPermissionRequest> _addPermissionValidator;

        private readonly ILogger<PermissionService> _logger;

        public PermissionService(IBaseRepository<PermissionEntity> permissionRepository,
            IValidator<AddPermissionRequest> addPermissionValidator, ILogger<PermissionService> logger)
        {
            _permissionRepository = permissionRepository;
            _addPermissionValidator = addPermissionValidator;
            _logger = logger;
        }

        public Result Add(AddPermissionRequest addPermission)
        {
            ValidationResult validationResult = _addPermissionValidator.Validate(addPermission);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(AddPermissionRequest)} model");
                return Result.Fail(validationResult.Errors);
            }

            BaseSpecification<PermissionEntity> baseSpecification = new BaseSpecification<PermissionEntity>();
            baseSpecification.AddFilter(x => x.Name.ToUpper() == addPermission.Name.ToUpper());

            bool permissionAlreadyExist = _permissionRepository.Exist(baseSpecification);
            if(permissionAlreadyExist)
            {
                _logger.LogError($"Permission with that name already exist");
                return Result.Fail("permission_already_exists", "Permission already exists");
            }

            PermissionEntity permission = new PermissionEntity(
                name: addPermission.Name,
                description: addPermission.Description);

            bool addResult = _permissionRepository.Add(permission);
            if(!addResult)
            {
                _logger.LogError($"Failed to add Permission. PermissionName {permission.Name}");
                return Result.Fail("failed_to_add_permission", "Failed to add permission");
            }

            return Result.Ok();
        }
    }
}
