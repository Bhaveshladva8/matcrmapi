﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using matcrm.data.Extensions;
using matcrm.data.Models.MollieModel.Connect;
using matcrm.data.Context;
using matcrm.service.Services.Client;

namespace matcrm.service.Services.Mollie.Client {
    public class ConnectClient : BaseMollieClient, IConnectClient {
        public const string AuthorizeEndPoint = "https://www.mollie.com/oauth2/authorize";
        public const string TokenEndPoint = "https://api.mollie.nl/oauth2/";
        private readonly string _clientId;
        private readonly string _clientSecret;

        public ConnectClient(string clientId, string clientSecret, HttpClient httpClient = null): base(httpClient, ConnectClient.TokenEndPoint) {
            if (string.IsNullOrWhiteSpace(clientId)) {
                throw new ArgumentNullException(nameof(clientId));
            }

            if (string.IsNullOrWhiteSpace(clientSecret)) {
                throw new ArgumentNullException(nameof(clientSecret));
            }
            
            this._clientSecret = OneClappContext.MollieSecretKey;
            this._clientId = OneClappContext.MollieClientId;
        }

        public string GetAuthorizationUrl(string state, List<string> scopes, string redirectUri = null, bool forceApprovalPrompt = false, string locale = null) {
            var parameters = new Dictionary<string, string> {
                {"client_id", this._clientId},
                {"state", state},
                {"scope", string.Join(" ", scopes)},
                {"response_type", "code"},
                {"approval_prompt", forceApprovalPrompt ? "force" : "auto"}
            };
            parameters.AddValueIfNotNullOrEmpty("redirect_uri", redirectUri);
            parameters.AddValueIfNotNullOrEmpty("locale", locale);

            return AuthorizeEndPoint + parameters.ToQueryString();
        }

        public async Task<TokenResponse> GetAccessTokenAsync(TokenRequest request) {
            return await this.PostAsync<TokenResponse>("tokens", request).ConfigureAwait(false);
        }

        public async Task RevokeTokenAsync(RevokeTokenRequest request) {
            await this.DeleteAsync("tokens", request).ConfigureAwait(false);
        }

        protected override HttpRequestMessage CreateHttpRequest(HttpMethod method, string relativeUri, HttpContent content = null) {
            HttpRequestMessage httpRequest = new HttpRequestMessage(method, new Uri(new Uri(ConnectClient.TokenEndPoint), relativeUri));
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", this.Base64Encode($"{this._clientId}:{this._clientSecret}"));
            httpRequest.Content = content;

            return httpRequest;
        }

        private string Base64Encode(string value) {
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }
    }

    public interface IConnectClient {
        /// <summary>
        ///     Constructs the Authorize URL for the Authorize endpoint from the parameters
        /// </summary>
        /// <param name="state">A random string generated by your app to prevent CSRF attacks.</param>
        /// <param name="scopes">
        ///     A space separated list of permissions your app requires. Refer to OAuth: Permissions for more
        ///     information about the available scopes.
        /// </param>
        /// <param name="redirectUri">
        ///     The URL the merchant is sent back to once the request has been authorized. If given, it must
        ///     match the URL you set when registering your app.
        /// </param>
        /// <param name="forceApprovalPrompt">
        ///     This parameter can be set to force, to force showing the consent screen to the
        ///     merchant, even when it is not necessary
        /// </param>
        /// <param name="locale">
        ///     Allows you to preset the language to be used in the login / sign up / authorize flow if the merchant is not known by Mollie. 
        ///     When this parameter is omitted, the browser language will be used instead. 
        ///     You can provide any ISO 15897 locale, but the authorize flow currently only supports the following languages:
        ///     Possible values: en_US nl_NL nl_BE fr_FR fr_BE de_DE es_ES it_IT
        /// </param>
        /// <returns>The url to the mollie consent screen.</returns>
        string GetAuthorizationUrl(string state, List<string> scopes, string redirectUri = null,
            bool forceApprovalPrompt = false, string locale = null);

        /// <summary>
        /// Exchange the auth code received at the Authorize endpoint for an actual access token, with which you can
        /// communicate with the Mollie API. Or Refresh the accestoken
        /// </summary>
        /// <param name="request"></param>
        /// <returns>An token object.</returns>
        Task<TokenResponse> GetAccessTokenAsync(TokenRequest request);

        /// <summary>
        /// Revoke an access- or a refresh token. Once revoked the token can not be used anymore.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task RevokeTokenAsync(RevokeTokenRequest request);
    }
}