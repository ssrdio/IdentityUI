using FluentValidation;
using FluentValidation.Results;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Data;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Role.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Role
{
    internal class RoleService : IRoleService
    {
        private readonly RoleManager<RoleEntity> _roleManager;

        private readonly IRoleRepository _roleRepository;

        private readonly IValidator<NewRoleRequest> _newRoleValidator;
        private readonly IValidator<EditRoleRequest> _editRoleValidator;

        private readonly ILogger<RoleService> _logger;

        public RoleService(RoleManager<RoleEntity> roleManager, IRoleRepository roleRepository, IValidator<NewRoleRequest> newRoleRequest,
            IValidator<EditRoleRequest> editRoleValidator, ILogger<RoleService> logger)
        {
            _roleManager = roleManager;

            _roleRepository = roleRepository;

            _newRoleValidator = newRoleRequest;
            _editRoleValidator = editRoleValidator;

            _logger = logger;
        }

        public async Task<Result<string>> AddRole(NewRoleRequest newRoleRequest, string adminId)
        {
            ValidationResult validationResult = _newRoleValidator.Validate(newRoleRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogError($"Invalid NewRoleRequest. Admin {adminId}");
                return Result.Fail<string>(ResultUtils.ToResultError(validationResult.Errors));
            }

            RoleEntity role = new RoleEntity( 
                name: newRoleRequest.Name,
                description: newRoleRequest.Description,
                type: newRoleRequest.Type.Value);

            IdentityResult result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to add new role. Admin with id {adminId}");
                return Result.Fail<string>(ResultUtils.ToResultError(result.Errors));
            }

            role = await _roleManager.FindByNameAsync(newRoleRequest.Name);
            if (role == null)
            {
                _logger.LogError($"Failed to find new role with name {newRoleRequest.Name}. Admin with id {adminId}");
                return Result.Fail<string>("no_role", "No role");
            }

            return Result.Ok(role.Id);
        }

        public Result EditRole(string id, EditRoleRequest editRoleRequest, string adminId)
        {
            ValidationResult validationResult = _editRoleValidator.Validate(editRoleRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogError($"Invalid EditRoleRequest. admin {adminId}");
                return Result.Fail(ResultUtils.ToResultError(validationResult.Errors));
            }

            BaseSpecification<RoleEntity> roleSpecification = new BaseSpecification<RoleEntity>();
            roleSpecification.AddFilter(x => x.Id == id);

            RoleEntity role = _roleRepository.Get(roleSpecification);
            if(role == null)
            {
                _logger.LogError($"No role. Admin id {adminId}");
                return Result.Fail("no_role", "No Role");
            }

            role.Description = editRoleRequest.Description;

            bool result = _roleRepository.Update(role);
            if(!result)
            {
                _logger.LogError($"Failed to update role with id {id}. Admin {adminId}");
                return Result.Fail("error", "Error");
            }

            return Result.Ok();
        }
    }
}
