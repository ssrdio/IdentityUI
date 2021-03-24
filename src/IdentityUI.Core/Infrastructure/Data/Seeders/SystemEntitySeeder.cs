using SSRD.IdentityUI.Core.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Services.OpenIdConnect.Models;
using SSRD.IdentityUI.Core.Data.Models.Seed;
using SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect;
using SSRD.IdentityUI.Core.Interfaces.Services.OpenIdConnect;
using SSRD.IdentityUI.Core.Models;
using OpenIddict.Abstractions;
using SSRD.IdentityUI.Core.Services.OpenIdConnect;
using SSRD.IdentityUI.Core.Models.Options;
using Microsoft.Extensions.Options;
using Flurl;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Seeders
{
    internal class SystemEntitySeeder
    {
        private readonly IdentityDbContext _context;
        private readonly RoleManager<RoleEntity> _roleManager;

        private readonly IAddClientService _addClientService;
        private readonly IManageClientService _manageClientService;
        private readonly IClientScopeService _clientScopeService;

        private readonly IdentityUIOptions _identityUIOptions;

        private readonly ILogger<SystemEntitySeeder> _logger;

        public SystemEntitySeeder(
            IdentityDbContext context,
            RoleManager<RoleEntity> roleManager,
            IAddClientService addClientService,
            IManageClientService manageClientService,
            IClientScopeService clientScopeService,
            IOptions<IdentityUIOptions> identityUIOptions,
            ILogger<SystemEntitySeeder> logger)
        {
            _context = context;

            _roleManager = roleManager;
            
            _addClientService = addClientService;
            _manageClientService = manageClientService;
            _clientScopeService = clientScopeService;

            _identityUIOptions = identityUIOptions.Value;

            _logger = logger;
        }

        public Task SeedIdentityUI()
        {
            return Seed(new List<PermissionSeedModel>(), new List<RoleSeedModel>(), new List<ClientSeedModel>());
        }

        public async Task Seed(List<PermissionSeedModel> permissionSeedModels, List<RoleSeedModel> roleSeedModels, List<ClientSeedModel> clientSeedModels)
        {
            _logger.LogInformation($"Seeding system entities");

            permissionSeedModels.AddRange(IdentityUIPermissions.ALL_DATA);
            roleSeedModels.AddRange(IdentityUIRoles.ALL_DATA);

            await SeedRoles(roleSeedModels);
            SeedPermissions(permissionSeedModels);
            SeedRolePermissions(roleSeedModels);
            SeedRoleAssigments(roleSeedModels);

            ClientSeedModel clientSeedModel = new ClientSeedModel
            {
                Name = "IdentityUI Admin",
                ClientId = OpenIdConnectConstants.IDENTITY_UI_CLIENT_ID,
                Description = "Client for IdentityUI Admin",
                Endpoints = new List<string>
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization.Replace(OpenIddictConstants.Permissions.Prefixes.Endpoint, ""),
                    OpenIddictConstants.Permissions.Endpoints.Token.Replace(OpenIddictConstants.Permissions.Prefixes.Endpoint, ""),
                    OpenIddictConstants.Permissions.Endpoints.Logout.Replace(OpenIddictConstants.Permissions.Prefixes.Endpoint, ""),
                },
                GrantTypes = new List<string>
                {
                    OpenIddictConstants.GrantTypes.AuthorizationCode,
                },
                PostLogoutUrls = new List<string>
                {
                    _identityUIOptions.BasePath.AppendPathSegment("signout-callback-oidc"),
                },
                RedirectUrls = new List<string>
                {
                    _identityUIOptions.BasePath.AppendPathSegment("signin-oidc"),
                },
                RequireConsent = false,
                RequirePkce = true,
                ResponseTypes = new List<string>
                {
                    OpenIddictConstants.ResponseTypes.Code
                },
                Scopes = new List<string>
                {
                    OpenIddictConstants.Scopes.OpenId,
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Scopes.Phone,
                    OpenIddictConstants.Scopes.Roles,
                    OpenIdConnectConstants.Scopes.Permissions,
                },
                Secret = null,
            };

            clientSeedModels.Add(clientSeedModel);

            List<string> requiredScopes = clientSeedModels
                .SelectMany(x => x.Scopes)
                .Distinct()
                .ToList();

            await SeedClientScopes(requiredScopes);
            await SeedClients(clientSeedModels);
        }

        private void SeedPermissions(List<PermissionSeedModel> permissionSeedModels)
        {
            List<PermissionEntity> existingPermissions = _context.Permissions.ToList();

            IEnumerable<PermissionEntity> missingPermissions = permissionSeedModels
                .Where(x => !existingPermissions.Any(c => c.Name.ToUpper() == x.Name.ToUpper()))
                .Select(x => x.ToEntity());

            _context.Permissions.AddRange(missingPermissions);

            existingPermissions = _context.Permissions.ToList();
            foreach (PermissionSeedModel permission in permissionSeedModels)
            {
                bool exists = existingPermissions
                    .Where(x => x.Name == permission.Name)
                    .Any();
                if (!exists)
                {
                    _logger.LogCritical($"Failed to seed Permission. Permission name {permission.Name}");
                    throw new Exception("Failed to seed Permission");
                }
            }
        }

        private async Task SeedRoles(List<RoleSeedModel> roleSeedModels)
        {
            List<RoleEntity> existingRoles = _context.Roles.ToList();

            IEnumerable<RoleEntity> missingRoles = roleSeedModels
                .Where(x => !existingRoles.Any(c => c.NormalizedName == x.Name.ToUpper()))
                .Select(x => x.ToEntity());

            foreach(RoleEntity roleEntity in missingRoles)
            {
                IdentityResult createRoleResult = await _roleManager.CreateAsync(roleEntity);
                if (!createRoleResult.Succeeded)
                {
                    _logger.LogCritical($"Failed to seed Role. Name {roleEntity.Name}, Error {string.Join(" ", createRoleResult.Errors.Select(x => x.Description))}");
                    throw new Exception($"Failed to seed Role.");
                }
            }
        }

        private void SeedRolePermissions(List<RoleSeedModel> roleSeedModels)
        {
            List<RoleEntity> roles = _context.Roles
                .Include(x => x.Permissions)
                .ThenInclude(x => x.Permission)
                .ToList();

            List<PermissionEntity> permissions = _context.Permissions.ToList();

            List<PermissionRoleEntity> permissionRoleEntities = new List<PermissionRoleEntity>();

            foreach(RoleSeedModel role in roleSeedModels)
            {
                RoleEntity roleEntity = roles
                    .Where(x => x.NormalizedName == role.Name.ToUpper())
                    .SingleOrDefault();

                if(roleEntity == null)
                {
                    _logger.LogCritical($"No role Role. Name {role.Name}");
                    throw new Exception($"No Role.");
                }

                foreach (string permission in role.Permissions)
                {
                    bool exists = roleEntity.Permissions
                        .Where(x => x.Permission.Name.ToUpper() == permission.ToUpper())
                        .Any();
                    if (exists)
                    {
                        continue;
                    }

                    PermissionEntity permissionEntity = permissions
                        .Where(x => x.Name.ToUpper() == permission.ToUpper())
                        .SingleOrDefault();
                    if (permissionEntity == null)
                    {
                        _logger.LogCritical($"Missing role permission. Role: {role.Name}, Permission {permission}");
                        throw new Exception("Missing role permission");
                    }

                    PermissionRoleEntity permissionRoleEntity = new PermissionRoleEntity(
                        permissionEntity.Id,
                        roleEntity.Id);

                    permissionRoleEntities.Add(permissionRoleEntity);
                }
            }

            _context.PermissionRole.AddRange(permissionRoleEntities);
            int addPermissionRoleChanges = _context.SaveChanges();
            if (addPermissionRoleChanges != permissionRoleEntities.Count())
            {
                _logger.LogCritical($"Failed to seed role permissions");
                throw new Exception("Failed to seed role permissions");
            }
        }

        private void SeedRoleAssigments(List<RoleSeedModel> roleSeedModels)
        {
            List<RoleEntity> roles = _context.Roles
                .Include(x => x.CanAssigne)
                .ThenInclude(x => x.Role)
                .ToList();

            List<RoleAssignmentEntity> roleAssignmentEntities = new List<RoleAssignmentEntity>();

            foreach(RoleSeedModel role in roleSeedModels)
            {
                if(!role.RoleAssignments.Any())
                {
                    continue;
                }

                RoleEntity roleEntity = roles
                    .Where(x => x.NormalizedName == role.Name.ToUpper())
                    .SingleOrDefault();

                if (roleEntity == null)
                {
                    _logger.LogCritical($"No role Role. Name {roleEntity.Name}");
                    throw new Exception($"No Role.");
                }

                foreach(string assigment in role.RoleAssignments)
                {
                    bool exists = roles
                        .Where(x => x.CanAssigne.Any(c => c.CanAssigneRole.NormalizedName == assigment.ToUpper()))
                        .Any();
                    if(exists)
                    {
                        continue;
                    }

                    RoleEntity assigneRoleEntity = roles
                        .Where(x => x.NormalizedName == assigment.ToUpper())
                        .SingleOrDefault();

                    if (assigneRoleEntity == null)
                    {
                        _logger.LogCritical($"No assign role. Name {assigneRoleEntity.Name}");
                        throw new Exception($"No assign Role.");
                    }

                    if (assigneRoleEntity.NormalizedName == roleEntity.NormalizedName)
                    {
                        _logger.LogCritical($"Role can not assign self. Role name {roleEntity.Name}");
                        throw new Exception($"Role can not assign self");
                    }

                    if(assigneRoleEntity.Type != RoleTypes.Group)
                    {
                        _logger.LogCritical($"Wrong role type for role assignment. Role name {roleEntity.Name}");
                        throw new Exception($"Wrong role type for role assignment");
                    }

                    RoleAssignmentEntity roleAssignmentEntity = new RoleAssignmentEntity(
                        roleEntity.Id,
                        assigneRoleEntity.Id);

                    roleAssignmentEntities.Add(roleAssignmentEntity);
                }
            }

            _context.RoleAssignments.AddRange(roleAssignmentEntities);
            int roleAssignmentsChanges = _context.SaveChanges();
            if (roleAssignmentsChanges != roleAssignmentEntities.Count())
            {
                _logger.LogCritical($"Failed to seed role assignments");
                throw new Exception("Failed to seed role assignments");
            }
        }

        private async Task SeedClientScopes(List<string> scopes)
        {
            List<ClientScopeEntity> scopeEntities = await _context.ClientScopes.ToListAsync();

            if(!scopeEntities.Where(x => x.Name == OpenIddictConstants.Scopes.OpenId).Any())
            {
                AddClientScopeModel addClientScopeModel = new AddClientScopeModel(
                    name: OpenIddictConstants.Scopes.OpenId,
                    displayName: "openid",
                    description: "Indicates that client application intends to use Openid connect to verify user identity");

                Result result = await _clientScopeService.Add(addClientScopeModel);
                if(result.Failure)
                {
                    _logger.LogError($"Failed to add system client scope openid");
                }
            }

            if(!scopeEntities.Where(x => x.Name == OpenIddictConstants.Scopes.OfflineAccess).Any())
            {
                AddClientScopeModel addClientScopeModel = new AddClientScopeModel(
                    name: OpenIddictConstants.Scopes.OfflineAccess,
                    displayName: "offline_access",
                    description: "This scope is only allowed if the client supports Refresh token grant");

                Result result = await _clientScopeService.Add(addClientScopeModel);
                if (result.Failure)
                {
                    _logger.LogError($"Failed to add system client scope offline_access");
                }
            }

            if(!scopeEntities.Where(x => x.Name == OpenIddictConstants.Scopes.Profile).Any())
            {
                AddClientScopeModel addClientScopeModel = new AddClientScopeModel(
                    name: OpenIddictConstants.Scopes.Profile,
                    displayName: "profile",
                    description: "Request access to user default claims");

                Result result = await _clientScopeService.Add(addClientScopeModel);
                if (result.Failure)
                {
                    _logger.LogError($"Failed to add system client scope profile");
                }
            }

            if (!scopeEntities.Where(x => x.Name == OpenIddictConstants.Scopes.Email).Any())
            {
                AddClientScopeModel addClientScopeModel = new AddClientScopeModel(
                    name: OpenIddictConstants.Scopes.Email,
                    displayName: "email",
                    description: "Request access user email");

                Result result = await _clientScopeService.Add(addClientScopeModel);
                if (result.Failure)
                {
                    _logger.LogError($"Failed to add system client scope profile");
                }
            }

            if (!scopeEntities.Where(x => x.Name == OpenIddictConstants.Scopes.Email).Any())
            {
                AddClientScopeModel addClientScopeModel = new AddClientScopeModel(
                    name: OpenIddictConstants.Scopes.Phone,
                    displayName: "phone",
                    description: "Request access user phone number");

                Result result = await _clientScopeService.Add(addClientScopeModel);
                if (result.Failure)
                {
                    _logger.LogError($"Failed to add system client scope phone");
                }
            }

            if (!scopeEntities.Where(x => x.Name == OpenIddictConstants.Scopes.Roles).Any())
            {
                AddClientScopeModel addClientScopeModel = new AddClientScopeModel(
                    name: "roles",
                    displayName: "roles",
                    description: "Request access user roles");

                Result result = await _clientScopeService.Add(addClientScopeModel);
                if (result.Failure)
                {
                    _logger.LogError($"Failed to add system client scope roles");
                }
            }

            if (!scopeEntities.Where(x => x.Name == OpenIdConnectConstants.Scopes.Permissions).Any())
            {
                AddClientScopeModel addClientScopeModel = new AddClientScopeModel(
                    name: "permissions",
                    displayName: "permissions",
                    description: "Request access user permissions");

                Result result = await _clientScopeService.Add(addClientScopeModel);
                if (result.Failure)
                {
                    _logger.LogError($"Failed to add system client scope permissions");
                }
            }

            scopeEntities = await _context.ClientScopes.ToListAsync();

            foreach (string scope in scopes)
            {
                if (!scopeEntities.Where(x => x.Name == scope).Any())
                {
                    AddClientScopeModel addClientScopeModel = new AddClientScopeModel(
                        name: scope,
                        displayName: scope,
                        description: null);

                    Result result = await _clientScopeService.Add(addClientScopeModel);
                    if (result.Failure)
                    {
                        _logger.LogError($"Failed to add system client scope {scope}");
                    }
                }
            }
        }

        private async Task SeedClients(List<ClientSeedModel> clientSeedModels)
        {
            foreach (ClientSeedModel client in clientSeedModels)
            {
                ClientEntity clientEntity = await _context.Clients
                    .Where(x => x.ClientId == client.ClientId)
                    .SingleOrDefaultAsync();

                if (clientEntity != null)
                {
                    continue;
                }

                AddCustomClientModel addCustomClientModel = new AddCustomClientModel
                {
                    ClientId = client.ClientId,
                    Name = client.Name
                };

                Result<IdStringModel> addClientResult = await _addClientService.AddCustomClient(addCustomClientModel);
                if (addClientResult.Failure)
                {
                    _logger.LogError($"Failed to seed client. Client {client.ClientId}");
                    continue;
                }

                UpdateClientModel updateClientModel = new UpdateClientModel
                {
                    Endpoints = client.Endpoints,
                    GrantTypes = client.GrantTypes,
                    Name = client.Name,
                    RedirectUrls = client.RedirectUrls,
                    PostLogoutUrls = client.PostLogoutUrls,
                    RequirePkce = client.RequirePkce,
                    RequireConsent = client.RequireConsent,
                    ResponseTypes = client.ResponseTypes,
                };

                Result updateResult = await _manageClientService.Update(addClientResult.Value.Id, updateClientModel);
                if (updateResult.Failure)
                {
                    _logger.LogError($"Failed to update seed Client. Client {client.ClientId}");
                    continue;
                }

                if (!string.IsNullOrEmpty(client.Secret))
                {
                    Result setSecretResult = await _manageClientService.GenerateNewClientSecret(addClientResult.Value.Id, client.Secret);
                    if (setSecretResult.Failure)
                    {
                        _logger.LogError($"Failed to set client secret. Client {client.ClientId}");
                    }
                }

                if(client.Scopes != null)
                {
                    ManageClientScopesModel manageClientScopesModel = new ManageClientScopesModel
                    {
                        Scopes = client.Scopes,
                    };

                    Result addScopesResult = await _manageClientService.AddScopes(addClientResult.Value.Id, manageClientScopesModel);
                    if(addScopesResult.Failure)
                    {
                        _logger.LogError($"Failed to add scopes to seed");
                    }
                }
            }
        }
    }
}
