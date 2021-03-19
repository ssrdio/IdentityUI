using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect;
using SSRD.IdentityUI.Core.Interfaces.Services.OpenIdConnect;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect
{
    public class ClientConsentService : IClientConsentService
    {
        private const string FAILED_TO_UPDATE_CONSENT_STATUS = "failed_to_update_consent_status";

        private readonly IBaseDAO<ClientConsentEntity> _consentDAO;

        private readonly ILogger<ClientConsentService> _logger;

        public ClientConsentService(IBaseDAO<ClientConsentEntity> consentDAO, ILogger<ClientConsentService> logger)
        {
            _consentDAO = consentDAO;
            _logger = logger;
        }

        private async Task<Result> UpdateStatus(List<string> consentIds, string status)
        {
            var specification = SpecificationBuilder
                .Create<ClientConsentEntity>()
                .Where(x => consentIds.Contains(x.Id))
                .Where(x => x.Status != status)
                .Build();

            List<ClientConsentEntity> consents = await _consentDAO.Get(specification);

            if(consents.Count == 0)
            {
                _logger.LogInformation($"All consents already are in correct status");
                return Result.Ok();
            }

            foreach(ClientConsentEntity consent in consents)
            {
                consent.Status = status;
            }

            bool updateStatus = await _consentDAO.UpdateRange(consents);
            if(!updateStatus)
            {
                _logger.LogError($"Failed to update consent status to {status}.");
                return Result.Fail(FAILED_TO_UPDATE_CONSENT_STATUS);
            }

            return Result.Ok();
        }

        public Task<Result> SetValidStatus(List<string> consentIds)
        {
            return UpdateStatus(consentIds, OpenIddictConstants.Statuses.Valid);
        }

        public Task<Result> SetRevokedStatus(List<string> consentIds)
        {
            return UpdateStatus(consentIds, OpenIddictConstants.Statuses.Revoked);
        }
    }
}
