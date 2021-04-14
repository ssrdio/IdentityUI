using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using SSRD.IdentityUI.Account.Areas.Account.Models.Consent;
using SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Services;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Services
{
    public class ConsentDataService : IConsentDataService
    {
        private readonly IBaseDAO<ClientConsentEntity> _clientConsentDAO;
        private readonly IIdentityUIUserInfoService _identityUIUserInfoService;
        private readonly ILogger<ConsentDataService> _logger;

        public ConsentDataService(
            IBaseDAO<ClientConsentEntity> clientConsentDAO,
            IIdentityUIUserInfoService identityUIUserInfoService,
            ILogger<ConsentDataService> logger)
        {
            _clientConsentDAO = clientConsentDAO;
            _identityUIUserInfoService = identityUIUserInfoService;
            _logger = logger;
        }

        public async Task<Result<DataTableResult<ConsentModel>>> Get(DataTableRequest dataTableRequest)
        {
            string userId = _identityUIUserInfoService.GetUserId();

            //TODO: ignore clients that does not require consent
            ISelectSpecificationBuilder<ClientConsentEntity, ConsentModel> specification = SpecificationBuilder
                .Create<ClientConsentEntity>()
                .Where(x => x.Subject == userId)
                .Where(x => x.Status == OpenIddictConstants.Statuses.Valid)
                .Select(x => new ConsentModel(
                    x.Id,
                    x.Scopes,
                    x.Type,
                    x.Application.DisplayName,
                    x.CreationDate));

            DataTableResult<ConsentModel> dataTableResult = await _clientConsentDAO.Get(specification, dataTableRequest);

            return Result.Ok(dataTableResult);
        }
    }
}
