using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Interfaces.Services.OpenIdConnect;
using SSRD.IdentityUI.Core.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect
{
    public class IdentityUIOpenIdService : IIdentityUIOpenIdService
    {
        private readonly IBaseDAO<AppUserEntity> _userDAO;

        private readonly IIdentityUIUserInfoService _identityUIUserInfoService;

        private readonly IdentityUIClaimOptions _identityUIClaimOptions;

        private readonly ILogger<IdentityUIOpenIdService> _logger;

        public IdentityUIOpenIdService(
            IBaseDAO<AppUserEntity> userDAO,
            IIdentityUIUserInfoService identityUIUserInfoService,
            IOptions<IdentityUIClaimOptions> identityUIClaimOptions,
            ILogger<IdentityUIOpenIdService> logger)
        {
            _userDAO = userDAO;
            _identityUIUserInfoService = identityUIUserInfoService;
            _identityUIClaimOptions = identityUIClaimOptions.Value;
            _logger = logger;

        }

        public async Task<Result<Dictionary<string, object>>> GetUserInfo()
        {
            string userId = _identityUIUserInfoService.GetUserId();

            IBaseSpecification<AppUserEntity, UserData> specification = SpecificationBuilder
                .Create<AppUserEntity>()
                .Where(x => x.Id == userId)
                .Select(x => new UserData(
                    x.Id,
                    x.UserName,
                    _identityUIUserInfoService.HasScope(OpenIddictConstants.Scopes.Email) ? x.Email : null,
                    _identityUIUserInfoService.HasScope(OpenIddictConstants.Scopes.Email) ? x.EmailConfirmed : false,
                    _identityUIUserInfoService.HasScope(OpenIddictConstants.Scopes.Phone) ? x.PhoneNumber : null,
                    _identityUIUserInfoService.HasScope(OpenIddictConstants.Scopes.Phone) ? x.PhoneNumberConfirmed : false,
                    _identityUIUserInfoService.HasScope(OpenIddictConstants.Scopes.Profile) ? x.FirstName : null,
                    _identityUIUserInfoService.HasScope(OpenIddictConstants.Scopes.Profile) ? x.LastName : null,
                    !_identityUIUserInfoService.HasScope("groups") ? null : x.Groups
                        .Select(c => new GroupData(
                            c.Group.Id,
                            c.Group.Name,
                            c.Role.Name,
                            !_identityUIUserInfoService.HasScope("permissions") ? null : c.Role.Permissions.Select(v => v.Permission.Name))),
                    !_identityUIUserInfoService.HasScope("roles") ? null : x.UserRoles
                        .Where(x => x.Role.Type == RoleTypes.Global)
                        .Select(c => new RoleData(
                            c.Role.Name,
                            !_identityUIUserInfoService.HasScope("permissions") ? null : c.Role.Permissions.Select(v => v.Permission.Name)))
                    ))
                .Build();

            UserData user = await _userDAO.SingleOrDefault(specification);

            Dictionary<string, object> userInfo = new Dictionary<string, object>();

            userInfo.Add(_identityUIClaimOptions.UserId, user.Id);
            userInfo.Add(_identityUIClaimOptions.Username, user.Username);

            if (!string.IsNullOrEmpty(user.Email))
            {
                userInfo.Add(_identityUIClaimOptions.Email, user.Email);
            }

            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                userInfo.Add(_identityUIClaimOptions.PhoneNumber, user.PhoneNumber);
            }

            if (!string.IsNullOrEmpty(user.FirstName))
            {
                userInfo.Add(_identityUIClaimOptions.FirstName, user.FirstName);
            }

            if (!string.IsNullOrEmpty(user.LastName))
            {
                userInfo.Add(_identityUIClaimOptions.LastName, user.LastName);
            }

            if (!string.IsNullOrEmpty(user.FirstName) || !string.IsNullOrEmpty(user.LastName))
            {
                userInfo.Add(_identityUIClaimOptions.Name, $"{user.FirstName} {user.LastName}");
            }

            if(user.Roles != null && user.Roles.Count() > 0)
            {
                List<string> roles = user.Roles
                    .Select(x => x.Name)
                    .ToList();

                userInfo.Add(_identityUIClaimOptions.Role, roles);

                List<string> permissions = user.Roles
                    .Where(x => x.Permissions != null)
                    .SelectMany(x => x.Permissions)
                    .Distinct()
                    .ToList();

                if(permissions.Count() > 0)
                {
                    userInfo.Add(_identityUIClaimOptions.Permission, permissions);
                }
            }

            return Result.Ok(userInfo);
        }
    }

    public class RoleData
    {
        public string Name { get; set; }
        public IEnumerable<string> Permissions { get; set; }

        public RoleData(string name, IEnumerable<string> permissions)
        {
            Name = name;
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
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public IEnumerable<GroupData> Groups { get; set; }
        public IEnumerable<RoleData> Roles { get; set; }

        public UserData(
            string id,
            string username,
            string email,
            bool emailConfirmed,
            string phoneNumber,
            bool phoneNumberConfirmed,
            string firstName,
            string lastName,
            IEnumerable<GroupData> groups,
            IEnumerable<RoleData> roles)
        {
            Id = id;
            Username = username;
            Email = email;
            EmailConfirmed = emailConfirmed;
            PhoneNumber = phoneNumber;
            PhoneNumberConfirmed = phoneNumberConfirmed;
            FirstName = firstName;
            LastName = lastName;
            Groups = groups;
            Roles = roles;
        }
    }
}
