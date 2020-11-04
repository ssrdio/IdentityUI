using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Services.Identity;
using System;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Group
{
    public class GroupStore : IGroupStore
    {
        public const string GROUP_NOT_FOUND = "group_not_found";

        private readonly IBaseRepository<GroupEntity> _groupRepository;
        private readonly IBaseDAO<GroupEntity> _groupDAO;

        private readonly IIdentityUIUserInfoService _identityUIUserInfoService;

        private readonly ILogger<GroupStore> _logger;

        public GroupStore(IBaseRepository<GroupEntity> groupRepository,
            IBaseDAO<GroupEntity> groupDAO,
            IIdentityUIUserInfoService identityUIUserInfoService,
            ILogger<GroupStore> logger)
        {
            _groupRepository = groupRepository;
            _groupDAO = groupDAO;
            _identityUIUserInfoService = identityUIUserInfoService;
            _logger = logger;
        }

        private TSpecification ApplayGroupFilter<TSpecification>(TSpecification specification)
            where TSpecification : BaseSpecification<GroupEntity>
        {
            if (_identityUIUserInfoService.HasPermission(IdentityUIPermissions.IDENTITY_UI_CAN_MANAGE_GROUPS))
            {
            }
            else if(_identityUIUserInfoService.GetGroupId() != null)
            {
                specification.AddFilter(x => x.Id == _identityUIUserInfoService.GetGroupId());
            }
            else
            {
                specification.AddFilter(x => false);
            }

            return specification;
        }

        [Obsolete("Use Any(string id)")]
        public Core.Models.Result.Result Exists(BaseSpecification<GroupEntity> baseSpecification)
        {
            baseSpecification = ApplayGroupFilter(baseSpecification);

            bool groupExist = _groupRepository.Exist(baseSpecification);
            if (!groupExist)
            {
                _logger.LogError($"No group.");
                return Core.Models.Result.Result.Fail("no_group", "No Group");
            }

            return Core.Models.Result.Result.Ok();
        }

        [Obsolete("Use Any(IBaseSpecification<GroupEntity, GroupEntity> specification)")]
        public Core.Models.Result.Result Exists(string groupId)
        {
            BaseSpecification<GroupEntity> baseSpecification = new BaseSpecification<GroupEntity>();
            baseSpecification.AddFilter(x => x.Id == groupId);

            return Exists(baseSpecification);
        }

        [Obsolete("Use SingleOrDefault(string id)")]
        public Core.Models.Result.Result<GroupEntity> Get(string id)
        {
            BaseSpecification<GroupEntity> baseSpecification = new BaseSpecification<GroupEntity>();
            baseSpecification.AddFilter(x => x.Id == id);

            _logger.LogInformation($"Getting Group. GroupId {id}");

            return Get(baseSpecification);
        }

        [Obsolete("Use SingleOrDefault(IBaseSpecification<GroupEntity, GroupEntity> specification)")]
        public Core.Models.Result.Result<GroupEntity> Get(BaseSpecification<GroupEntity> baseSpecification)
        {
            baseSpecification = ApplayGroupFilter(baseSpecification);

            GroupEntity group = _groupRepository.SingleOrDefault(baseSpecification);
            if (group == null)
            {
                _logger.LogError($"No group. GroupId");
                return Core.Models.Result.Result.Fail<GroupEntity>("no_group", "No Group");
            }

            return Core.Models.Result.Result.Ok(group);
        }

        [Obsolete("Use SingleOrDefault<TValue>(IBaseSpecification<GroupEntity, TValue> specification)")]
        public T Get<T>(SelectSpecification<GroupEntity, T> selectSpecification)
        {
            selectSpecification = ApplayGroupFilter(selectSpecification);

            return _groupRepository.SingleOrDefault(selectSpecification);
        }

        private IBaseSpecification<GroupEntity, TValue> ApplayGroupFilter<TValue>(IBaseSpecification<GroupEntity, TValue> specification)
        {
            if (_identityUIUserInfoService.HasPermission(IdentityUIPermissions.IDENTITY_UI_CAN_MANAGE_GROUPS))
            {
            }
            else if (_identityUIUserInfoService.GetGroupId() != null)
            {
                specification.Filters.Add(x => x.Id == _identityUIUserInfoService.GetGroupId());
            }
            else
            {
                specification.Filters.Add(x => false);
            }

            return specification;
        }

        public Task<Result> Any(string id)
        {
            IBaseSpecification<GroupEntity, GroupEntity> specification = SpecificationBuilder
                .Create<GroupEntity>()
                .WithId(id)
                .Build();

            return Any(specification);
        }

        public async Task<Result> Any(IBaseSpecification<GroupEntity, GroupEntity> specification)
        {
            ApplayGroupFilter(specification);

            bool exists = await _groupDAO.Exist(specification);
            if(!exists)
            {
                _logger.LogError($"Group not found");
                return Result.Fail(GROUP_NOT_FOUND);
            }

            return Result.Ok();
        }

        public Task<Result<GroupEntity>> SingleOrDefault(string id)
        {
            IBaseSpecification<GroupEntity, GroupEntity> specification = SpecificationBuilder
                .Create<GroupEntity>()
                .WithId(id)
                .Select()
                .Build();

            return SingleOrDefault(specification);
        }

        public async Task<Result<TValue>> SingleOrDefault<TValue>(IBaseSpecification<GroupEntity, TValue> specification)
        {
            ApplayGroupFilter(specification);

            TValue group = await _groupDAO.SingleOrDefault(specification);
            if(group == null)
            {
                _logger.LogError($"Group not found");
                return Result.Fail<TValue>(GROUP_NOT_FOUND);
            }

            return Result.Ok(group);
        }
    }
}
