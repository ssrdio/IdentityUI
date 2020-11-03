using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Admin.Interfaces;
using SSRD.IdentityUI.Admin.Models.Group;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Services
{
    public class GroupAttributeDataService : IGroupAttributeDataService
    {
        private readonly IBaseDAO<GroupAttributeEntity> _groupAttributeDAO;

        private readonly IValidator<DataTableRequest> _dataTableValidator;
        private readonly ILogger<GroupAttributeDataService> _logger;

        public GroupAttributeDataService(
            IBaseDAO<GroupAttributeEntity> groupAttributeDAO, IValidator<DataTableRequest> dataTableValidator,
            ILogger<GroupAttributeDataService> logger)
        {
            _groupAttributeDAO = groupAttributeDAO;
            _dataTableValidator = dataTableValidator;
            _logger = logger;
        }

        public async Task<Result<DataTableResult<GroupAttributeTableModel>>> Get(string groupId, DataTableRequest dataTableRequest)
        {
            ValidationResult validationResult = _dataTableValidator.Validate(dataTableRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(dataTableRequest)} model");
                return Result.Fail<DataTableResult<GroupAttributeTableModel>>(validationResult.ToResultError());
            }

            ISelectSpecificationBuilder<GroupAttributeEntity, GroupAttributeTableModel> specificationBuilder = SpecificationBuilder
                .Create<GroupAttributeEntity>()
                .Where(x => x.GroupId == groupId)
                .SearchByKey(dataTableRequest.Search)
                .OrderByDessending(x => x._CreatedDate)
                .Select(x => new GroupAttributeTableModel(
                    x.Id,
                    x.Key,
                    x.Value));

            IBaseSpecification<GroupAttributeEntity, GroupAttributeTableModel> countSpecification = specificationBuilder.Build();
            IBaseSpecification<GroupAttributeEntity, GroupAttributeTableModel> dataSpecification = specificationBuilder
                .Paginate(dataTableRequest.Start, dataTableRequest.Length)
                .Build();

            int count = await _groupAttributeDAO.Count(countSpecification);
            List<GroupAttributeTableModel> data = await _groupAttributeDAO.Get(countSpecification);

            DataTableResult<GroupAttributeTableModel> dataTableResult = new DataTableResult<GroupAttributeTableModel>(
                draw: dataTableRequest.Draw,
                recordsTotal: count,
                recordsFiltered: count,
                data: data);

            return Result.Ok(dataTableResult);
        }
    }
}
