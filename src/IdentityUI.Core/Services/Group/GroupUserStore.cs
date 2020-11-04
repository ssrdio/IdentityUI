using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Services.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Group
{
    internal class GroupUserStore : IGroupUserStore
    {
        private const string NO_GROUP_USER = "no_group_user";
        private const string NO_PERMISSION = "no_permission";

        private readonly IBaseRepository<GroupUserEntity> _groupUserRepository;
        private readonly IBaseRepository<RoleEntity> _roleRepository;
        private readonly IBaseRepository<RoleAssignmentEntity> _roleAssignmentRepository;

        private readonly IBaseDAO<GroupUserEntity> _groupUserDAO;
        private readonly IBaseDAO<RoleAssignmentEntity> _roleAssignmentDAO;
        private readonly IBaseDAO<RoleEntity> _roleDAO;

        private readonly IdentityUIClaimOptions _identityUIClaimOptions;

        private readonly IIdentityUIUserInfoService _identityUIUserInfoService;
        private readonly ILogger<GroupUserStore> _logger;

        public GroupUserStore(
            IBaseRepository<GroupUserEntity> groupUserRepository,
            IBaseRepository<RoleEntity> roleRepository,
            IBaseRepository<RoleAssignmentEntity> roleAssignmentRepository,
            IBaseDAO<GroupUserEntity> groupUserDAO,
            IBaseDAO<RoleAssignmentEntity> roleAssignmentDAO,
            IBaseDAO<RoleEntity> roleDAO,
            IOptions<IdentityUIClaimOptions> identityUIClaimOptions,
            IIdentityUIUserInfoService identityUIUserInfoService,
            ILogger<GroupUserStore> logger)
        {
            _groupUserRepository = groupUserRepository;
            _roleRepository = roleRepository;
            _roleAssignmentRepository = roleAssignmentRepository;

            _groupUserDAO = groupUserDAO;
            _roleAssignmentDAO = roleAssignmentDAO;
            _roleDAO = roleDAO;

            _identityUIClaimOptions = identityUIClaimOptions.Value;

            _identityUIUserInfoService = identityUIUserInfoService;

            _logger = logger;
        }

        private TSpecification ApplayGroupUserFilter<TSpecification>(TSpecification specification)
            where TSpecification : BaseSpecification<GroupUserEntity>
        {
            if (_identityUIUserInfoService.HasPermission(IdentityUIPermissions.GROUP_CAN_SEE_USERS))
            {
            }
            else if (_identityUIUserInfoService.HasGroupPermission(IdentityUIPermissions.GROUP_CAN_SEE_USERS)
                    && _identityUIUserInfoService.GetGroupId() != null)
            {
                specification.AddFilter(x => x.GroupId == _identityUIUserInfoService.GetGroupId());
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

        private List<RoleListData> CanAssigneRolesOld()
        {
            string userId = _identityUIUserInfoService.GetUserId();
            string groupId = _identityUIUserInfoService.GetGroupId();

            bool hasGlobalAccess = _identityUIUserInfoService.HasPermission(IdentityUIPermissions.GROUP_CAN_MANAGE_ROLES);

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
            List<RoleListData> roles = CanAssigneRolesOld();
            roles.Add(new RoleListData(
                id: null,
                name: null));

            return roles;
        }

        public List<RoleListData> CanAssigneGroupRoles()
        {
            return CanAssigneRolesOld();
        }

        public Core.Models.Result.Result<GroupUserEntity> Get(BaseSpecification<GroupUserEntity> baseSpecification)
        {
            baseSpecification = ApplayGroupUserFilter(baseSpecification);

            GroupUserEntity groupUser = _groupUserRepository.SingleOrDefault(baseSpecification);
            if (groupUser == null)
            {
                _logger.LogError($"No GroupUser. No GroupUser");
                return Core.Models.Result.Result.Fail<GroupUserEntity>("no_group_user", "No GroupUser");
            }

            return Core.Models.Result.Result.Ok(groupUser);
        }

        public Core.Models.Result.Result<GroupUserEntity> Get(string userId, string groupId)
        {
            BaseSpecification<GroupUserEntity> getGroupUserSpecification = new BaseSpecification<GroupUserEntity>();
            getGroupUserSpecification.AddFilter(x => x.UserId == userId);
            getGroupUserSpecification.AddFilter(x => x.GroupId == groupId);

            return Get(getGroupUserSpecification);
        }

        public Core.Models.Result.Result<GroupUserEntity> Get(long id)
        {
            BaseSpecification<GroupUserEntity> getGroupUserSpecification = new BaseSpecification<GroupUserEntity>();
            getGroupUserSpecification.AddFilter(x => x.Id == id);

            return Get(getGroupUserSpecification);
        }

        public Core.Models.Result.Result Exists(long id)
        {
            BaseSpecification<GroupUserEntity> baseSpecification = new BaseSpecification<GroupUserEntity>();
            baseSpecification.AddFilter(x => x.Id == id);

            return Exists(baseSpecification);
        }

        public Core.Models.Result.Result Exists(BaseSpecification<GroupUserEntity> baseSpecification)
        {
            baseSpecification = ApplayGroupUserFilter(baseSpecification);

            bool exists = _groupUserRepository.Exist(baseSpecification);
            if (!exists)
            {
                _logger.LogError($"No GroupUser. No GroupUser");
                return Core.Models.Result.Result.Fail<GroupUserEntity>("no_group_user", "No GroupUser");
            }

            return Core.Models.Result.Result.Ok();
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
            if (_identityUIUserInfoService.HasGroupPermission(IdentityUIPermissions.GROUP_CAN_MANAGE_ROLES))
            {
                return true;
            }

            return false;
        }

        private IBaseSpecification<GroupUserEntity, TData> ApplayGroupUserFilter<TData>(IBaseSpecification<GroupUserEntity, TData> specification)
        {
            if (_identityUIUserInfoService.HasPermission(IdentityUIPermissions.GROUP_CAN_SEE_USERS))
            {
            }
            else if (_identityUIUserInfoService.HasGroupPermission(IdentityUIPermissions.GROUP_CAN_SEE_USERS)
                    && _identityUIUserInfoService.GetGroupId() != null)
            {
                specification.Filters.Add(x => x.GroupId == _identityUIUserInfoService.GetGroupId());
            }
            else
            {
                specification.Filters.Add(x => false);
            }

            return specification;
        }

        private async Task<List<RoleListData>> GetRoleAssignments(string userId, string groupId)
        {
            IBaseSpecification<GroupUserEntity, RoleListData> getGroupRoleSpecification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => x.UserId == userId)
                .Where(x => x.GroupId == groupId)
                .Select(x => new RoleListData(
                    x.Role.Id,
                    x.Role.Name))
                .Build();

            RoleListData role = await _groupUserDAO.SingleOrDefault(getGroupRoleSpecification);
            if (role == null)
            {
                _logger.LogInformation($"User has no groupRole. UserId {userId}, GroupId {groupId}");
                return new List<RoleListData>();
            }

            IBaseSpecification<RoleAssignmentEntity, RoleListData> getRoleAssigmentsSpecification = SpecificationBuilder
                .Create<RoleAssignmentEntity>()
                .Where(x => x.RoleId == role.Id)
                .Select(x => new RoleListData(
                    x.CanAssigneRole.Id,
                    x.CanAssigneRole.Name))
                .Build();

            List<RoleListData> canAssigneRoles = await _roleAssignmentDAO.Get(getRoleAssigmentsSpecification);

            if (!canAssigneRoles.Any(x => x.Id == role.Id))
            {
                canAssigneRoles.Add(role);
            }

            return canAssigneRoles;
        }

        private Task<List<RoleListData>> GetAllGroupRolesAsync()
        {
            IBaseSpecification<RoleEntity, RoleListData> specification = SpecificationBuilder
                .Create<RoleEntity>()
                .Where(x => x.Type == Data.Enums.Entity.RoleTypes.Group)
                .Select(x => new RoleListData(
                    x.Id,
                    x.Name))
                .Build();

            return _roleDAO.Get(specification);
        }

        private async Task<List<RoleListData>> GetRoleAssignments()
        {
            string userId = _identityUIUserInfoService.GetUserId();
            string groupId = _identityUIUserInfoService.GetGroupId();

            bool hasGlobalAccess = _identityUIUserInfoService.HasPermission(IdentityUIPermissions.GROUP_CAN_MANAGE_ROLES);

            List<RoleListData> roles;

            if (hasGlobalAccess)
            {
                roles = await GetAllGroupRolesAsync();
            }
            else
            {
                roles = await GetRoleAssignments(userId, groupId);
            }

            return roles;
        }

        public Task<Result> Any(long id)
        {
            IBaseSpecification<GroupUserEntity, GroupUserEntity> specification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => x.Id == id)
                .Build();

            return Any(specification);
        }

        public async Task<Result> Any(IBaseSpecification<GroupUserEntity, GroupUserEntity> specification)
        {
            specification = ApplayGroupUserFilter(specification);

            bool exists = await _groupUserDAO.Exist(specification);
            if(!exists)
            {
                _logger.LogError($"GroupUser not found");
                return Result.Fail(NO_GROUP_USER);
            }

            return Result.Ok();
        }

        public Task<Result<GroupUserEntity>> SingleOrDefault(long id)
        {
            IBaseSpecification<GroupUserEntity, GroupUserEntity> specification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => x.Id == id)
                .Select()
                .Build();

            return SingleOrDefault(specification);
        }

        public Task<Result<GroupUserEntity>> SingleOrDefault(string userId, string groupId)
        {
            IBaseSpecification<GroupUserEntity, GroupUserEntity> specification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => x.UserId == userId)
                .Where(x => x.GroupId == groupId)
                .Select()
                .Build();

            return SingleOrDefault(specification);
        }

        public async Task<Result<TData>> SingleOrDefault<TData>(IBaseSpecification<GroupUserEntity, TData> specification)
        {
            specification = ApplayGroupUserFilter(specification);

            TData groupUser = await _groupUserDAO.SingleOrDefault(specification);
            if(groupUser == null)
            {
                _logger.LogError($"Group User not found");
                return Result.Fail<TData>(NO_GROUP_USER);
            }

            return Result.Ok(groupUser);
        }

        public Task<List<TData>> Get<TData>(IBaseSpecification<GroupUserEntity, TData> specification)
        {
            specification = ApplayGroupUserFilter(specification);

            return _groupUserDAO.Get(specification); 
        }

        public Task<int> Count<TData>(IBaseSpecification<GroupUserEntity, TData> specification)
        {
            specification = ApplayGroupUserFilter(specification);

            return _groupUserDAO.Count(specification);
        }

        public Task<List<RoleListData>> CanManageRoles()
        {
            return GetRoleAssignments();
        }

        public Task<List<RoleListData>> CanAssigneRoles()
        {
            return GetRoleAssignments();
        }

        public async Task<Result> CanManageUser(long groupUserId)
        {
            Result<GroupUserEntity> getGroupUserResult = await SingleOrDefault(groupUserId);
            if (getGroupUserResult.Failure)
            {
                return Result.Fail(getGroupUserResult);
            }

            GroupUserEntity groupUser = getGroupUserResult.Value;

            return await CanManageUser(groupUser);
        }

        public async Task<Result> CanManageUser(GroupUserEntity groupUser)
        {
            List<RoleListData> rolesList = await CanManageRoles();
            if (!rolesList.Any(x => x.Id == groupUser.RoleId))
            {
                _logger.LogError($"User does not have a permission to remove user. GroupUserId {groupUser.Id}");
                return Result.Fail(NO_PERMISSION);
            }

            return Result.Ok();
        }
    }
}
