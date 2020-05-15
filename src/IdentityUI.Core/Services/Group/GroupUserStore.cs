using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Group
{
    internal class GroupUserStore : IGroupUserStore
    {
        private readonly IBaseRepository<GroupUserEntity> _groupUserRepository;
        private readonly IBaseRepository<RoleEntity> _roleRepository;
        private readonly IBaseRepository<RoleAssignmentEntity> _roleAssignmentRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GroupUserStore> _logger;

        public GroupUserStore(IBaseRepository<GroupUserEntity> groupUserRepository, IBaseRepository<RoleEntity> roleRepository,
            IBaseRepository<RoleAssignmentEntity> roleAssignmentRepository, IHttpContextAccessor httpContextAccessor,
            ILogger<GroupUserStore> logger)
        {
            _groupUserRepository = groupUserRepository;
            _roleRepository = roleRepository;
            _roleAssignmentRepository = roleAssignmentRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private TSpecification ApplayGroupUserFilter<TSpecification>(TSpecification specification)
            where TSpecification : BaseSpecification<GroupUserEntity>
        {
            if (_httpContextAccessor.HttpContext.User.HasPermission(IdentityUIPermissions.GROUP_CAN_SEE_USERS))
            {
            }
            else if (_httpContextAccessor.HttpContext.User.HasGroupPermission(IdentityUIPermissions.GROUP_CAN_SEE_USERS))
            {
                specification.AddFilter(x => x.GroupId == _httpContextAccessor.HttpContext.User.GetGroupId());
            }
            else
            {
                specification.AddFilter(x => false);
            }

            return specification;
        }

        private List<RoleListData> GetRoleAssignmes(string userId, string groupId)
        {
            SelectSpecification<GroupUserEntity, RoleListData> getGroupRoleSpecification = new SelectSpecification<GroupUserEntity, RoleListData>();
            getGroupRoleSpecification.AddFilter(x => x.UserId == userId);
            getGroupRoleSpecification.AddFilter(x => x.GroupId == groupId);

            getGroupRoleSpecification.AddSelect(x => new RoleListData(
                x.Role.Id,
                x.Role.Name));

            RoleListData groupRole = _groupUserRepository.SingleOrDefault(getGroupRoleSpecification);
            if (groupRole == null)
            {
                _logger.LogInformation($"User has no groupRole. UserId {userId}, GroupId {groupId}");
                return new List<RoleListData>();
            }

            SelectSpecification<RoleAssignmentEntity, RoleListData> getRoleAssignmesSpecification = new SelectSpecification<RoleAssignmentEntity, RoleListData>();
            getRoleAssignmesSpecification.AddFilter(x => x.RoleId == groupRole.Id);

            getRoleAssignmesSpecification.AddSelect(x => new RoleListData(
                x.CanAssigneRole.Id,
                x.CanAssigneRole.Name));

            List<RoleListData> canAssigneRoles = _roleAssignmentRepository.GetList(getRoleAssignmesSpecification);

            if (!canAssigneRoles.Any(x => x.Id == groupRole.Id))
            {
                canAssigneRoles.Add(groupRole);
            }

            return canAssigneRoles;
        }

        private List<RoleListData> GetAllGroupRoles()
        {
            SelectSpecification<RoleEntity, RoleListData> selectSpecification = new SelectSpecification<RoleEntity, RoleListData>();
            selectSpecification.AddFilter(x => x.Type == Data.Enums.Entity.RoleTypes.Group);

            selectSpecification.AddSelect(x => new RoleListData(
                x.Id,
                x.Name));

            List<RoleListData> roleListData = _roleRepository.GetList(selectSpecification);

            return roleListData;
        }

        private List<RoleListData> CanAssigneRoles()
        {
            string userId = _httpContextAccessor.HttpContext.User.GetUserId();
            string groupId = _httpContextAccessor.HttpContext.User.GetGroupId();

            bool hasGlobalAccess = _httpContextAccessor.HttpContext.User.HasPermission(IdentityUIPermissions.GROUP_CAN_MANAGE_ROLES);

            List<RoleListData> roles;

            if (hasGlobalAccess)
            {
                roles = GetAllGroupRoles();
            }
            else
            {
                roles = GetRoleAssignmes(userId, groupId);
            }

            return roles;
        }

        public List<RoleListData> CanManageGroupRoles()
        {
            List<RoleListData> roles = CanAssigneRoles();
            roles.Add(new RoleListData(
                id: null,
                name: null));

            return roles;
        }

        public List<RoleListData> CanAssigneGroupRoles()
        {
            return CanAssigneRoles();
        }

        public Result<GroupUserEntity> Get(BaseSpecification<GroupUserEntity> baseSpecification)
        {
            baseSpecification = ApplayGroupUserFilter(baseSpecification);

            GroupUserEntity groupUser = _groupUserRepository.SingleOrDefault(baseSpecification);
            if (groupUser == null)
            {
                _logger.LogError($"No GroupUser. No GroupUser");
                return Result.Fail<GroupUserEntity>("no_group_user", "No GroupUser");
            }

            return Result.Ok(groupUser);
        }

        public Result<GroupUserEntity> Get(string userId, string groupId)
        {
            BaseSpecification<GroupUserEntity> getGroupUserSpecification = new BaseSpecification<GroupUserEntity>();
            getGroupUserSpecification.AddFilter(x => x.UserId == userId);
            getGroupUserSpecification.AddFilter(x => x.GroupId == groupId);

            return Get(getGroupUserSpecification);
        }

        public Result<GroupUserEntity> Get(long id)
        {
            BaseSpecification<GroupUserEntity> getGroupUserSpecification = new BaseSpecification<GroupUserEntity>();
            getGroupUserSpecification.AddFilter(x => x.Id == id);

            return Get(getGroupUserSpecification);
        }

        public Result Exists(long id)
        {
            BaseSpecification<GroupUserEntity> baseSpecification = new BaseSpecification<GroupUserEntity>();
            baseSpecification.AddFilter(x => x.Id == id);

            return Exists(baseSpecification);
        }

        public Result Exists(BaseSpecification<GroupUserEntity> baseSpecification)
        {
            baseSpecification = ApplayGroupUserFilter(baseSpecification);

            bool exists = _groupUserRepository.Exist(baseSpecification);
            if (!exists)
            {
                _logger.LogError($"No GroupUser. No GroupUser");
                return Result.Fail<GroupUserEntity>("no_group_user", "No GroupUser");
            }

            return Result.Ok();
        }

        public T Get<T>(SelectSpecification<GroupUserEntity, T> selectSpecification)
        {
            selectSpecification = ApplayGroupUserFilter(selectSpecification);

            return _groupUserRepository.SingleOrDefault(selectSpecification);
        }

        public PaginatedData<T> GetPaginated<T>(PaginationSpecification<GroupUserEntity, T> paginationSpecification)
        {
            paginationSpecification = ApplayGroupUserFilter(paginationSpecification);

            return _groupUserRepository.GetPaginated(paginationSpecification);
        }

        public bool CanChangeOwnRole()
        {
            if (_httpContextAccessor.HttpContext.User.HasPermission(IdentityUIPermissions.GROUP_CAN_MANAGE_ROLES))
            {
                return true;
            }

            return false;
        }
    }
}
