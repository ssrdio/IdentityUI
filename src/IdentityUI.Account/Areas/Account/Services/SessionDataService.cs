using Microsoft.Extensions.Logging;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using SSRD.IdentityUI.Account.Areas.Account.Models.Session;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Data.Specifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Services
{
    public class SessionDataService : ISessionDataService
    {
        private readonly IBaseDAO<SessionEntity> _sessionDAO;
        private readonly IIdentityUIUserInfoService _identityUIUserInfoService;
        private readonly ILogger<SessionDataService> _logger;

        public SessionDataService(
            IBaseDAO<SessionEntity> sessionDAO,
            IIdentityUIUserInfoService identityUIUserInfoService,
            ILogger<SessionDataService> logger)
        {
            _sessionDAO = sessionDAO;
            _identityUIUserInfoService = identityUIUserInfoService;
            _logger = logger;
        }

        public async Task<Result<DataTableResult<SessionModel>>> Get(DataTableRequest dataTableRequest)
        {
            string userId = _identityUIUserInfoService.GetUserId();

            IBaseSpecificationBuilder<SessionEntity> specification = SpecificationBuilder
                .Create<SessionEntity>()
                .Where(x => x.UserId == userId)
                .OrderByDessending(x => x.LastAccess);

            DataTableResult<SessionEntity> sessionEntities = await _sessionDAO.Get(specification, dataTableRequest);

            UAParser.Parser userAgentParser = UAParser.Parser.GetDefault(new UAParser.ParserOptions { MatchTimeOut = TimeSpan.FromSeconds(1) });

            List<SessionModel> sessionModels = new List<SessionModel>();

            foreach (SessionEntity sessionEntity in sessionEntities.Data)
            {
                string userAgent = null;
                string os = null;
                string device = null;

                if (sessionEntity.UserAgent != null)
                {
                    try
                    {
                        UAParser.ClientInfo clientInfo = userAgentParser.Parse(sessionEntity.UserAgent);

                        userAgent = clientInfo.UA.Family;
                        os = clientInfo.OS.Family;
                        device = clientInfo.Device.Family;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to parse UserAgent. SessionId {sessionEntity.Id}");
                    }
                }

                SessionModel sessionModel = new SessionModel(
                    id: sessionEntity.Id,
                    ip: sessionEntity.Ip,
                    userAgent: userAgent,
                    os: os,
                    device: device,
                    lastAccess: sessionEntity.LastAccess.UtcDateTime,
                    created: sessionEntity._CreatedDate.HasValue ? sessionEntity._CreatedDate.Value.UtcDateTime : new DateTime().ToUniversalTime());

                sessionModels.Add(sessionModel);
            }

            DataTableResult<SessionModel> dataTableResult = new DataTableResult<SessionModel>(
                draw: dataTableRequest.Start,
                recordsFiltered: sessionEntities.RecordsFiltered,
                recordsTotal: sessionEntities.RecordsTotal,
                data: sessionModels);

            return Result.Ok(dataTableResult);
        }

    }
}
