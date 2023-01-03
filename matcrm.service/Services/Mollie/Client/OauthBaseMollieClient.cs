using System;
using System.Net.Http;
using matcrm.service.Services.Client;

namespace matcrm.service.Services.Mollie.Client {
    public class OauthBaseMollieClient : BaseMollieClient {
        public OauthBaseMollieClient(string oauthAccessToken, HttpClient httpClient = null) : base(oauthAccessToken, httpClient) {
            if (string.IsNullOrWhiteSpace(oauthAccessToken)) {
                throw new ArgumentNullException(nameof(oauthAccessToken), "Mollie API key cannot be empty");
            }

            this.ValidateApiKeyIsOauthAccesstoken(true);
        }
    }
}