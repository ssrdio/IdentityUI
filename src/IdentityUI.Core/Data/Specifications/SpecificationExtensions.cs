using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Data.Specifications
{
    public static class SpecificationExtensions
    {
        public static IBaseSpecificationBuilder<TEntity> Paginate<TEntity>(
            this IBaseSpecificationBuilder<TEntity> builder,
            DataTableRequest dataTableRequest)
        {
            builder = builder
                .Paginate(dataTableRequest.Start, dataTableRequest.Length);

            return builder;
        }

        public static ISelectSpecificationBuilder<TEntity, TData> Paginate<TEntity, TData>(
            this ISelectSpecificationBuilder<TEntity, TData> builder,
            DataTableRequest dataTableRequest)
        {
            builder = builder
                .Paginate(dataTableRequest.Start, dataTableRequest.Length);

            return builder;
        }

        public static async Task<DataTableResult<TEntity>> Get<TEntity>(
            this IBaseDAO<TEntity> baseDAO,
            IBaseSpecificationBuilder<TEntity> builder,
            DataTableRequest dataTableRequest) where TEntity : class
        {
            IBaseSpecification<TEntity, TEntity> countSpecification = builder.Build();
            IBaseSpecification<TEntity, TEntity> selectSpecification = builder
                .Paginate(dataTableRequest)
                .Build();

            int count = await baseDAO.Count(countSpecification);
            List<TEntity> data = await baseDAO.Get(selectSpecification);

            DataTableResult<TEntity> dataTableResult = new DataTableResult<TEntity>(
                draw: dataTableRequest.Draw,
                recordsTotal: count,
                recordsFiltered: count,
                data: data);

            return dataTableResult;
        }

        public static async Task<DataTableResult<TData>> Get<TEntity, TData>(
            this IBaseDAO<TEntity> baseDAO,
            ISelectSpecificationBuilder<TEntity, TData> builder,
            DataTableRequest dataTableRequest) where TEntity : class
        {
            IBaseSpecification<TEntity, TData> countSpecification = builder.Build();
            IBaseSpecification<TEntity, TData> selectSpecification = builder
                .Paginate(dataTableRequest)
                .Build();

            int count = await baseDAO.Count(countSpecification);
            List<TData> data = await baseDAO.Get(selectSpecification);

            DataTableResult<TData> dataTableResult = new DataTableResult<TData>(
                draw: dataTableRequest.Draw,
                recordsTotal: count,
                recordsFiltered: count,
                data: data);

            return dataTableResult;
        }

        public static async Task<Select2Result<Select2Item>> Get<TEntity>(
            this IBaseDAO<TEntity> baseDAO,
            ISelectSpecificationBuilder<TEntity, string> builder,
            Select2Request select2Request) where TEntity : class
        {
            IBaseSpecification<TEntity, string> countSpecification = builder.Build();
            IBaseSpecification<TEntity, string> selectSpecification = builder
                .Paginate(select2Request.GetPageStart(), select2Request.GetPageLenght())
                .Build();

            int count = await baseDAO.Count(countSpecification);
            List<string> items = await baseDAO.Get(selectSpecification);

            Select2Result<Select2Item> select2Result = new Select2Result<Select2Item>(
                results: items
                    .Select(x => new Select2Item(
                        id: x?.ToString(),
                        text: x?.ToString()))
                    .ToList(),
                pagination: select2Request.IsMore(count));

            return select2Result;
        }
    }
}
