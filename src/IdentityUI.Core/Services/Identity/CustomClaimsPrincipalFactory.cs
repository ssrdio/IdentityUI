using SSRD.IdentityUI.Core.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Data.Enums.Entity;

namespace SSRD.IdentityUI.Core.Services.Identity
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUserEntity, RoleEntity>
    {
        private readonly IBaseRepositoryAsync<AppUserEntity> _userRepository;

        public CustomClaimsPrincipalFactory(UserManager<AppUserEntity> userManager, RoleManager<RoleEntity> roleManager,
            IOptions<IdentityOptions> identityOptions, IBaseRepositoryAsync<AppUserEntity> userRepository) : base(userManager, roleManager, identityOptions)
        {
            _userRepository = userRepository;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(AppUserEntity user)
        {
            SelectSpecification<AppUserEntity, UserData> userDataSpecification = new SelectSpecification<AppUserEntity, UserData>();
            userDataSpecification.AddFilter(x => x.Id == user.Id);
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

            if(userData == null)
            {
                throw new Exception("no user");
            }

            ClaimsIdentity claimsIdentity = userData.ToClaimIdentity(Options.ClaimsIdentity.SecurityStampClaimType);

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            if (!string.IsNullOrEmpty(user.SessionCode))
            {
                ((ClaimsIdentity)claimsPrincipal.Identity).AddClaim(new Claim(IdentityUIClaims.SESSION_CODE, user.SessionCode));
            }

            return claimsPrincipal;
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

            public ClaimsIdentity ToClaimIdentity(string securityStampClaimType)
            {
                ClaimsIdentity claimsIdentity = new ClaimsIdentity("Identity.Application");

                claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Id));
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, Username));
                claimsIdentity.AddClaim(new Claim(securityStampClaimType, SecurityStamp));

                if(Groups.Count() != 0 && Groups.Count() != 1)
                {
                    throw new Exception("multiply groups not supported");
                }

                GroupData group = Groups.FirstOrDefault();
                if(group != null)
                {
                    claimsIdentity.AddClaim(new Claim(IdentityUIClaims.GROUP_ID, group.GroupId));
                    claimsIdentity.AddClaim(new Claim(IdentityUIClaims.GROUP_NAME, group.GroupName));

                    if (!string.IsNullOrEmpty(group.RoleName))
                    {
                        claimsIdentity.AddClaim(new Claim(IdentityUIClaims.GROUP_ROLE, group.RoleName));
                    }

                    foreach (string permission in group.Permissions)
                    {
                        claimsIdentity.AddClaim(new Claim(IdentityUIClaims.GROUP_PERMISSION, permission));
                    }
                }

                foreach (RoleData role in Roles)
                {
                    if (role.Type == RoleTypes.Global)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Name));

                        foreach (string permission in role.Permissions)
                        {
                            claimsIdentity.AddClaim(new Claim(IdentityUIClaims.PERMISSION, permission));
                        }
                    }
                }

                return claimsIdentity;
            }
        }
    }
}
