using SSRD.IdentityUI.Core.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Models.Options;

namespace SSRD.IdentityUI.Core.Services.Identity
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUserEntity, RoleEntity>
    {
        private readonly IBaseRepositoryAsync<AppUserEntity> _userRepository;

        private readonly IdentityUIClaimOptions _identityUIClaimOptions;

        private readonly ILogger<CustomClaimsPrincipalFactory> _logger;

        public CustomClaimsPrincipalFactory(
            UserManager<AppUserEntity> userManager,
            RoleManager<RoleEntity> roleManager,
            IOptions<IdentityOptions> identityOptions,
            IOptions<IdentityUIClaimOptions> identityUIClaimOptions,
            IBaseRepositoryAsync<AppUserEntity> userRepository,
            ILogger<CustomClaimsPrincipalFactory> logger) : base(userManager, roleManager, identityOptions)
        {
            _logger = logger;
            _identityUIClaimOptions = identityUIClaimOptions.Value;
            _userRepository = userRepository;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(AppUserEntity user)
        {
            List<ClaimsIdentity> claimsIdentities = new List<ClaimsIdentity>();

            ClaimsIdentity userIdentity = await GetUserIdentity(user);

            claimsIdentities.Add(userIdentity);

            if(!string.IsNullOrEmpty(user.ImpersonatorId))
            {
                ClaimsIdentity impersonatorIdentity = await GetImpersonatorIdentity(user.ImpersonatorId);

                claimsIdentities.Add(impersonatorIdentity);
            }

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentities);

            return claimsPrincipal;
        }

        public virtual async Task<UserData> GetUserData(string userId)
        {
            SelectSpecification<AppUserEntity, UserData> userDataSpecification = new SelectSpecification<AppUserEntity, UserData>();
            userDataSpecification.AddFilter(x => x.Id == userId);
            userDataSpecification.AddSelect(x => new UserData(
                x.Id,
                x.UserName,
                x.SecurityStamp,
                x.Groups
                    .Select(c => new GroupData(
                        c.Group.Id,
                        c.Group.Name,
                        c.Role.Name,
                        c.Role.Permissions.Select(v => v.Permission.Name))),
                x.UserRoles.Select(c => new RoleData(
                    c.Role.Name,
                    c.Role.Type,
                    c.Role.Permissions.Select(v => v.Permission.Name)))
                ));

            UserData userData = await _userRepository.SingleOrDefault(userDataSpecification);
            if (userData == null)
            {
                _logger.LogError($"User does not exists. UserId {userId}");
                throw new Exception("no user");
            }

            return userData;
        }

        public virtual async Task<ClaimsIdentity> GetImpersonatorIdentity(string userId)
        {
            UserData userData = await GetUserData(userId);

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(_identityUIClaimOptions.ImpersonatorIdentityName);

            claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.ImpersonatorId, userData.Id));
            claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.ImpersonatorUsername, userData.Username));

            foreach (RoleData role in userData.Roles)
            {
                if (role.Type == RoleTypes.Global)
                {
                    claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.ImpersonatorRole, role.Name));

                    foreach (string permission in role.Permissions)
                    {
                        claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.ImpersonatorPermission, permission));
                    }
                }
            }

            GroupData group = userData.Groups.SingleOrDefault();
            if (group != null)
            {
                claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.ImpersonatorGroupId, group.GroupId));
                claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.ImpersonatorGroupName, group.GroupName));

                if (!string.IsNullOrEmpty(group.RoleName))
                {
                    claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.ImpersonatorGroupRole, group.RoleName));
                }

                foreach (string permission in group.Permissions)
                {
                    claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.ImpersonatorGroupPermission, permission));
                }
            }

            return claimsIdentity;
        }

        public virtual Task<ClaimsIdentity> GetUserIdentity(AppUserEntity appUser)
        {
            return GetUserIdentity(appUser.Id, appUser.SessionCode, appUser.ImpersonatorId);
        }

        public virtual async Task<ClaimsIdentity> GetUserIdentity(string userId, string sessionCode, string impersonatorId = null)
        {
            UserData userData = await GetUserData(userId);

            ClaimsIdentity claimsIdentity = new ClaimsIdentity("Identity.Application");

            claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.UserId, userData.Id));
            claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.Username, userData.Username));
            claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.SecurityStamp, userData.SecurityStamp));

            if (userData.Groups.Count() != 0 && userData.Groups.Count() != 1)
            {
                throw new Exception("multiply groups not supported");
            }

            GroupData group = userData.Groups.SingleOrDefault();
            if (group != null)
            {
                claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.GroupId, group.GroupId));
                claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.GroupName, group.GroupName));

                if (!string.IsNullOrEmpty(group.RoleName))
                {
                    claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.GroupRole, group.RoleName));
                }

                foreach (string permission in group.Permissions)
                {
                    claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.GroupPermission, permission));
                }
            }

            foreach (RoleData role in userData.Roles)
            {
                if (role.Type == RoleTypes.Global)
                {
                    claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.Role, role.Name));

                    foreach (string permission in role.Permissions)
                    {
                        claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.Permission, permission));
                    }
                }
            }

            if (!string.IsNullOrEmpty(sessionCode))
            {
                claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.SessionCode, sessionCode));
            }

            if (!string.IsNullOrEmpty(impersonatorId))
            {
                claimsIdentity.AddClaim(new Claim(_identityUIClaimOptions.ImpersonatorId, impersonatorId));
            }

            return claimsIdentity;
        }
    }

    public class RoleData
    {
        public string Name { get; set; }
        public RoleTypes Type { get; set; }
        public IEnumerable<string> Permissions { get; set; }

        public RoleData(string name, RoleTypes type, IEnumerable<string> permissions)
        {
            Name = name;
            Type = type;
            Permissions = permissions;
        }
    }

    public class GroupData
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string RoleName { get; set; }

        public IEnumerable<string> Permissions { get; set; }

        public GroupData(string groupId, string groupName, string roleName, IEnumerable<string> permissions)
        {
            GroupId = groupId;
            GroupName = groupName;
            RoleName = roleName;
            Permissions = permissions;
        }
    }

    public class UserData
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string SecurityStamp { get; set; }

        public IEnumerable<GroupData> Groups { get; set; }
        public IEnumerable<RoleData> Roles { get; set; }

        public UserData(string id, string username, string securityStamp, IEnumerable<GroupData> groups, IEnumerable<RoleData> roles)
        {
            Id = id;
            Username = username;
            SecurityStamp = securityStamp;
            Groups = groups;
            Roles = roles;
        }
    }
}
