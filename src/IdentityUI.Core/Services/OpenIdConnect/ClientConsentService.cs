using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Interfaces.Services.OpenIdConnect;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect
{
    public class ClientConsentService : IClientConsentService
    {
        private const string FAILED_TO_UPDATE_CONSENT_STATUS = "failed_to_update_consent_status";

        private const string CLIENT_CONSENT_NOT_FOUND = "client_consent_not_found";

        private readonly IBaseDAO<ClientConsentEntity> _consentDAO;
        private readonly IIdentityUIUserInfoService _identityUIUserInfoService;

        private readonly ILogger<ClientConsentService> _logger;

        public ClientConsentService(
            IBaseDAO<ClientConsentEntity> consentDAO,
            IIdentityUIUserInfoService identityUIUserInfoService,
            ILogger<ClientConsentService> logger)
        {
            _consentDAO = consentDAO;
            _identityUIUserInfoService = identityUIUserInfoService;
            _logger = logger;
        }

        private async Task<Result> UpdateStatus(List<string> consentIds, string status)
        {
            IBaseSpecification<ClientConsentEntity, ClientConsentEntity> specification = SpecificationBuilder
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

        public async Task<Result> RevokeConsent(string id)
        {
            string userId = _identityUIUserInfoService.GetUserId();

            IBaseSpecification<ClientConsentEntity, ClientConsentEntity> specification = SpecificationBuilder
                .Create<ClientConsentEntity>()
                .Where(x => x.Id == id)
                .Where(x => x.Subject == userId)
                .Where(x => x.Status == OpenIddictConstants.Statuses.Valid)
                .Build();

            ClientConsentEntity clientConsent = await _consentDAO.SingleOrDefault(specification);
            if(clientConsent == null)
            {
                _logger.LogError($"Client consent not found. CleintConsentId {id}, UserId {userId}");
                return Result.Fail(CLIENT_CONSENT_NOT_FOUND);
            }

            clientConsent.Status = OpenIddictConstants.Statuses.Revoked;

            bool updateResult = await _consentDAO.Update(clientConsent);
            if(!updateResult)
            {
                _logger.LogError($"Failed to update ClientConstent. CleintConsentId {id}, UserId {userId}");
                return Result.Fail(FAILED_TO_UPDATE_CONSENT_STATUS);
            }

            return Result.Ok();
        }
    }
}
