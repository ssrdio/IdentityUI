using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Dashboard;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Dashboard;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Services
{
    public class GroupAdminDashboardDataService : IGroupAdminDashboardService
    {
        private readonly IGroupUserStore _groupUserStore;

        private readonly ILogger<GroupAdminDashboardDataService> _logger;

        public GroupAdminDashboardDataService(IGroupUserStore groupUserStore, ILogger<GroupAdminDashboardDataService> logger)
        {
            _groupUserStore = groupUserStore;
            _logger = logger;
        }

        public async Task<GroupedStatisticsViewModel> GetIndexViewModel()
        {
            IBaseSpecification<GroupUserEntity, GroupUserEntity> getAllUsersSpecification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Build();

            int allUsers = await _groupUserStore.Count(getAllUsersSpecification);

            IBaseSpecification<GroupUserEntity, GroupUserEntity> getActiveUsersSpecification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => x.User.Sessions.Any())
                .Build();

            int activeUsers = await _groupUserStore.Count(getActiveUsersSpecification);

            IBaseSpecification<GroupUserEntity, GroupUserEntity> getUnconfirmedUsersSpecification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => !x.User.EmailConfirmed)
                .Build();

            int unconfirmedUsers = await _groupUserStore.Count(getUnconfirmedUsersSpecification);

            IBaseSpecification<GroupUserEntity, GroupUserEntity> getDisabledUsersSpecification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => !x.User.Enabled)
                .Build();

            int disabledUsers = await _groupUserStore.Count(getDisabledUsersSpecification);

            GroupedStatisticsViewModel viewModel = new GroupedStatisticsViewModel(
                usersCount: allUsers,
                activeUsersCount: activeUsers,
                unconfirmedUsersCount: unconfirmedUsers,
                disabledUsersCount: disabledUsers);

            return viewModel;
        }

        public async Task<Result<List<RegistrationsViewModel>>> GetRegistrationStatistics(DateTimeOffset from, DateTimeOffset to)
        {
            if (to < from)
            {
                _logger.LogError($"To is smaller than to. From {from.ToString()}, To {to.ToString()}");
                return Result.Fail<List<RegistrationsViewModel>>("error", "Error");
            }

            IBaseSpecification<GroupUserEntity, DateTimeOffset> specification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => x.User._CreatedDate != null)
                .Where(x => x.User._CreatedDate >= from)
                .Where(x => x.User._CreatedDate < to)
                .Select(x => x.User._CreatedDate.Value)
                .Build();

            List<DateTimeOffset> dateTimes = await _groupUserStore.Get(specification);


            List<RegistrationsViewModel> viewModel = dateTimes
                .GroupBy(x => new { x.UtcDateTime.Year, x.UtcDateTime.Month, x.UtcDateTime.Day })
                .Select(x => new RegistrationsViewModel(
                    date: new DateTime(x.Key.Year, x.Key.Month, x.Key.Day, 0, 0, 0, DateTimeKind.Utc).ToString("o"),
                    count: x.Count()))
                .ToList();

            DateTime dateTime = new DateTime(
                from.UtcDateTime.Year,
                from.UtcDateTime.Month,
                from.UtcDateTime.Day,
                0,
                0,
                0,
                DateTimeKind.Utc);

            for (; dateTime < to; dateTime = dateTime.AddDays(1))
            {
                bool exists = viewModel
                    .Where(x => x.Date == dateTime.ToString("o"))
                    .Any();

                if(!exists)
                {
                    viewModel.Add(new RegistrationsViewModel(
                        date: dateTime.ToString("o"),
                        count: 0));
                }
            }

            viewModel = viewModel
                .OrderBy(x => x.Date)
                .ToList();

            return Result.Ok(viewModel);
        }
    }
}
