using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace IdentityUI.Sample.Sms.Services
{
    public class TwilioSmsSender : ISmsSender
    {
        private readonly PhoneNumber _from;

        private readonly ILogger<TwilioSmsSender> _logger;

        public TwilioSmsSender(string sid, string token, string from, ILogger<TwilioSmsSender> logger)
        {
            TwilioClient.Init(sid, token);
            _from = new PhoneNumber(from);

            _logger = logger;
        }

        public Task<Result> Send(string to, string message)
        {
            try
            {
                MessageResource result = MessageResource.Create(
                    from: _from,
                    to: new PhoneNumber(to),
                    body: message);

                return Task.FromResult(Result.Ok());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send sms");
                return Task.FromResult(Result.Fail("twilio_error", "Sending SMS failed"));
            }
        }
    }
}
