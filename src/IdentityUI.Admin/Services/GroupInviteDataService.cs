using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Invite;
using SSRD.IdentityUI.Admin.Interfaces;
using SSRD.IdentityUI.Admin.Models.Group;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Services
{
    public class GroupInviteDataService : IGroupInviteDataService
    {
        private readonly IGroupUserStore _userStore;
        private readonly IBaseDAO<InviteEntity> _inviteDAO;

        private readonly IValidator<DataTableRequest> _dataTableRequestValidator;

        private readonly ILogger<GroupInviteDataService> _logger;

        public GroupInviteDataService(
            IGroupUserStore userStore,
            IBaseDAO<InviteEntity> inviteDAO,
            IValidator<DataTableRequest> dataTableRequestValidator,
            ILogger<GroupInviteDataService> logger)
        {
            _userStore = userStore;
            _inviteDAO = inviteDAO;
            _dataTableRequestValidator = dataTableRequestValidator;
            _logger = logger;
        }

        public async Task<Result<DataTableResult<GroupInviteTableModel>>> Get(string groupId, DataTableRequest dataTableRequest)
        {
            ValidationResult validationResult = _dataTableRequestValidator.Validate(dataTableRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(DataTableRequest)} model");
                return Result.Fail<DataTableResult<GroupInviteTableModel>>(validationResult.ToResultError());
            }

            ISelectSpecificationBuilder<InviteEntity, GroupInviteTableModel> specificationBuilder = SpecificationBuilder
                .Create<InviteEntity>()
                .Where(x => x.GroupId == groupId)
                .SearchByEmail(dataTableRequest.Search)
                .OrderByDessending(x => x._CreatedDate)
                .Select(x => new GroupInviteTableModel(
                    x.Id,
                    x.Email,
                    x.GroupRole.Name,
                    x.Status.GetDescription(),
                    x.ExpiresAt.ToString("o")));

            IBaseSpecification<InviteEntity, GroupInviteTableModel> countSpecification = specificationBuilder.Build();
            IBaseSpecification<InviteEntity, GroupInviteTableModel> dataSpecification = specificationBuilder
                .Paginate(dataTableRequest.Start, dataTableRequest.Length)
                .Build();

            int count = await _inviteDAO.Count(countSpecification);
            var data = await _inviteDAO.Get(dataSpecification);


            DataTableResult<GroupInviteTableModel> dataTableResult = new DataTableResult<GroupInviteTableModel>(
                draw: dataTableRequest.Draw,
                recordsTotal: count,
                recordsFiltered: count,
                data: data);

            return Result.Ok(dataTableResult);
        }
    }
}
