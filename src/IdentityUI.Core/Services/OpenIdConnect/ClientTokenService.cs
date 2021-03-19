using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect;
using SSRD.IdentityUI.Core.Interfaces.Services.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect
{
    public class ClientTokenService : IClientTokenService
    {
        private const string FAILED_TO_UPDATE_CLIENT_TOKENS_STATUSES = "failed_to_update_client_tokens_statuses";

        private readonly IBaseDAO<ClientTokenEntity> _clientTokenDAO;

        private readonly ILogger<ClientTokenService> _logger;

        public ClientTokenService(IBaseDAO<ClientTokenEntity> clientTokenDAO, ILogger<ClientTokenService> logger)
        {
            _clientTokenDAO = clientTokenDAO;
            _logger = logger;
        }

        private async Task<Result> UpdateStatus(List<string> tokenIds, string status)
        {
            IBaseSpecification<ClientTokenEntity, ClientTokenEntity> specification = SpecificationBuilder
                .Create<ClientTokenEntity>()
                .Where(x => tokenIds.Contains(x.Id))
                .Where(x => x.Status != status)
                .Build();

            List<ClientTokenEntity> tokens = await _clientTokenDAO.Get(specification);

            foreach(ClientTokenEntity token in tokens)
            {
                token.Status = status;
            }

            bool updateResult = await _clientTokenDAO.UpdateRange(tokens);
            if(!updateResult)
            {
                _logger.LogError($"Failed to update ClientTokens.");
                return Result.Fail(FAILED_TO_UPDATE_CLIENT_TOKENS_STATUSES);
            }

            return Result.Ok();
        }

        public Task<Result> RevokeTokens(List<string> tokenIds)
        {
            return UpdateStatus(tokenIds, OpenIddictConstants.Statuses.Revoked);
        }
    }
}
