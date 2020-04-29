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
using System.Security.Cryptography.Xml;
using SSRD.IdentityUI.Core.Data.Entities.Group;

namespace SSRD.IdentityUI.Core.Services.Identity
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUserEntity, RoleEntity>
    {
        private readonly IBaseRepository<UserRoleEntity> _userRoleRepository;
        private readonly IBaseRepository<GroupUserEntity> _groupUserRepository;

        public CustomClaimsPrincipalFactory(UserManager<AppUserEntity> userManager, RoleManager<RoleEntity> roleManager,
            IOptions<IdentityOptions> identityOptions, IBaseRepository<UserRoleEntity> userRoleRepository,
            IBaseRepository<GroupUserEntity> groupUserRepository) : base(userManager, roleManager, identityOptions)
        {
            _userRoleRepository = userRoleRepository;
            _groupUserRepository = groupUserRepository;
        }

        public override Task<ClaimsPrincipal> CreateAsync(AppUserEntity user)
        {
            //ClaimsPrincipal claimsPrincipal = await base.CreateAsync(user);

            //if (!string.IsNullOrEmpty(user.SessionCode))
            //{
            //    ((ClaimsIdentity)claimsPrincipal.Identity).AddClaim(new Claim(IdentityUIClaims.SESSION_CODE, user.SessionCode));
            //}

            //return claimsPrincipal;

            ClaimsIdentity claimsIdentity = new ClaimsIdentity("Identity.Application");
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            claimsIdentity.AddClaim(new Claim(Options.ClaimsIdentity.SecurityStampClaimType, user.SecurityStamp));

            SelectSpecification<GroupUserEntity, GroupData> getGroupIdSpecification = new SelectSpecification<GroupUserEntity, GroupData>();
            getGroupIdSpecification.AddFilter(x => x.UserId == user.Id);
            getGroupIdSpecification.AddSelect(x => new GroupData(
                x.Group.Id,
                x.Group.Name,
                x.Role.Name,
                x.Role.Permissions.Select(c => c.Permission.Name)));

            GroupData groupData = _groupUserRepository.Get(getGroupIdSpecification);
            if(groupData != null)
            {
                claimsIdentity.AddClaim(new Claim(IdentityUIClaims.GROUP_ID, groupData.GroupId));

                claimsIdentity.AddClaim(new Claim(IdentityUIClaims.GROUP_ROLE, groupData.RoleName));

                foreach (string permission in groupData.Permissions)
                {
                    claimsIdentity.AddClaim(new Claim(IdentityUIClaims.GROUP_PERMISSION, permission));
                }
            }

            SelectSpecification<UserRoleEntity, RoleData> selectSpecification = new SelectSpecification<UserRoleEntity, RoleData>();
            selectSpecification.AddFilter(x => x.UserId == user.Id);
            selectSpecification.AddSelect(x => new RoleData(
                x.Role.Name,
                x.Role.Type,
                x.Role.Permissions.Select(c => c.Permission.Name)));

            List<RoleData> roles = _userRoleRepository.GetList(selectSpecification);

            foreach (RoleData role in roles)
            {
                if (role.Type == RoleTypes.System)
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Name));

                    foreach (string permission in role.Permissions)
                    {
                        claimsIdentity.AddClaim(new Claim(IdentityUIClaims.PERMISSION, permission));
                    }
                }
                else if (role.Type == RoleTypes.Group)
                {
                    claimsIdentity.AddClaim(new Claim(IdentityUIClaims.GROUP_ROLE, role.Name));

                    foreach (string permission in role.Permissions)
                    {
                        claimsIdentity.AddClaim(new Claim(IdentityUIClaims.GROUP_PERMISSION, permission));
                    }
                }
            }

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            if (!string.IsNullOrEmpty(user.SessionCode))
            {
                ((ClaimsIdentity)claimsPrincipal.Identity).AddClaim(new Claim(IdentityUIClaims.SESSION_CODE, user.SessionCode));
            }

            return Task.FromResult(claimsPrincipal);
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
    }
}
