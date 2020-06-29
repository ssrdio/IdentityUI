using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Infrastructure.Services
{
    internal class NullSmsSender : ISmsSender
    {
        private readonly ILogger<NullSmsSender> _logger;

        public NullSmsSender(ILogger<NullSmsSender> logger)
        {
            _logger = logger;
        }

        public Task<Result> Send(string to, string message)
        {
            _logger.LogWarning($"NullSmsSender. Mail not sent");
            return Task.FromResult(Result.Ok());
        }
    }
}
