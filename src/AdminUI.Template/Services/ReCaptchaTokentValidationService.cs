using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSRD.AdminUI.Template.Models;
using SSRD.CommonUtils.Result;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SSRD.AdminUI.Template.Services
{
    public class ReCaptchaTokentValidationService
    {
        /// <summary>
        /// 0 = site secret
        /// 1 = token
        /// </summary>
        private const string VALIDATE_URL = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}";

        private const string INVALID_RECAPCHA_TOKEN = "invalid_recapcha_token";

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ReCaptchaOptions _reCaptchaOptions;

        private readonly ILogger<ReCaptchaTokentValidationService> _logger;

        public ReCaptchaTokentValidationService(
            IHttpClientFactory httpClientFactory,
            IOptions<ReCaptchaOptions> reCaptchaOptions,
            ILogger<ReCaptchaTokentValidationService> logger)
        {
            _httpClientFactory = httpClientFactory;

            _reCaptchaOptions = reCaptchaOptions.Value;

            _logger = logger;
        }

        public async Task<Result> Validate(string token, double requiredScore, string action)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();

            try
            {
                string url = string.Format(VALIDATE_URL, _reCaptchaOptions.SiteSecret, token);

                HttpResponseMessage httpResponse = await httpClient.GetAsync(url);
                string response = await httpResponse.Content.ReadAsStringAsync();

                if(httpResponse.IsSuccessStatusCode)
                {
#if NET_CORE2
                    ReCaptchaResponse reCaptchaResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ReCaptchaResponse>(response);
#elif NET_CORE3
                    ReCaptchaResponse reCaptchaResponse = System.Text.Json.JsonSerializer.Deserialize<ReCaptchaResponse>(response);
#endif

                    if(reCaptchaResponse.Success)
                    {
                        if(reCaptchaResponse.Score < requiredScore)
                        {
                            _logger.LogError($"ReCaptcha score is to low. Score {reCaptchaResponse.Score}, required score {requiredScore}");
                            return Result.Fail(INVALID_RECAPCHA_TOKEN);
                        }

                        if(!string.IsNullOrEmpty(action))
                        {
                            if(reCaptchaResponse.Action != action)
                            {
                                _logger.LogError($"ReCaptcha action do not match. Action {reCaptchaResponse.Action}, required action {action}");
                                return Result.Fail(INVALID_RECAPCHA_TOKEN);
                            }
                        }

                        return Result.Ok();
                    }

                    _logger.LogError($"ReCapcha validation service returned errors. Response {response}");
                }
                else
                {
                    _logger.LogError($"There was an error validation reCapcha token. HttpCode {httpResponse.StatusCode}, Content {response}");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "There was an exception during validation reCapcha token");
            }

            return Result.Fail(INVALID_RECAPCHA_TOKEN);
        }
    }
}
