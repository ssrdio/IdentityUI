using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.AdminUI.Template.Services
{
    public class ReCaptchaResponse
    {
#if NET_CORE2
        [Newtonsoft.Json.JsonProperty("success")]
#elif NET_CORE3
        [System.Text.Json.Serialization.JsonPropertyName("success")]
#endif
        public bool Success { get; set; }

#if NET_CORE2
        [Newtonsoft.Json.JsonProperty("challenge_ts")]
#elif NET_CORE3
        [System.Text.Json.Serialization.JsonPropertyName("challenge_ts")]
#endif
        public string ChallengeTs { get; set; }

#if NET_CORE2
        [Newtonsoft.Json.JsonProperty("hostname")]
#elif NET_CORE3
        [System.Text.Json.Serialization.JsonPropertyName("hostname")]
#endif
        public string Hostname { get; set; }

#if NET_CORE2
        [Newtonsoft.Json.JsonProperty("score")]
#elif NET_CORE3
        [System.Text.Json.Serialization.JsonPropertyName("score")]
#endif
        public double Score { get; set; }

#if NET_CORE2
        [Newtonsoft.Json.JsonProperty("action")]
#elif NET_CORE3
        [System.Text.Json.Serialization.JsonPropertyName("action")]
#endif
        public string Action { get; set; }

#if NET_CORE2
        [Newtonsoft.Json.JsonProperty("error-codes")]
#elif NET_CORE3
        [System.Text.Json.Serialization.JsonPropertyName("error-codes")]
#endif
        public List<string> ErrorCodes { get; set; }
    }
}