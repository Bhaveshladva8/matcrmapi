using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using matcrm.data.Extensions;
using matcrm.data.Models.MollieModel.List;
using matcrm.data.Models.MollieModel.PaymentLink.Request;
using matcrm.data.Models.MollieModel.PaymentLink.Response;
using matcrm.data.Models.MollieModel.Url;
using matcrm.service.Services.Client;

namespace matcrm.service.Services.Mollie.Client
{
    public class PaymentLinkClient : BaseMollieClient, IPaymentLinkClient
    {

        public PaymentLinkClient(string apiKey, HttpClient httpClient = null) : base(apiKey, httpClient) { }

        public async Task<PaymentLinkResponse> CreatePaymentLinkAsync(PaymentLinkRequest paymentLinkRequest)
        {
            if (!string.IsNullOrWhiteSpace(paymentLinkRequest.ProfileId) || paymentLinkRequest.Testmode.HasValue)
            {
                this.ValidateApiKeyIsOauthAccesstoken();
            }
            return await this.PostAsync<PaymentLinkResponse>($"payment-links", paymentLinkRequest).ConfigureAwait(false);
        }

        public async Task<PaymentLinkResponse> GetPaymentLinkAsync(string paymentLinkId, bool testmode = false)
        {
            if (testmode)
            {
                this.ValidateApiKeyIsOauthAccesstoken();
            }

            var queryParameters = this.BuildQueryParameters(
                testmode: testmode);

            return await this.GetAsync<PaymentLinkResponse>($"payment-links/{paymentLinkId}{queryParameters.ToQueryString()}").ConfigureAwait(false);
        }
         
        public async Task<PaymentLinkResponse> GetPaymentLinkAsync(UrlObjectLink<PaymentLinkResponse> url)
        {
            return await this.GetAsync(url).ConfigureAwait(false);
        }
         
        
        public async Task<ListResponse<PaymentLinkResponse>> GetPaymentLinkListAsync(UrlObjectLink<ListResponse<PaymentLinkResponse>> url)
        {
            return await this.GetAsync(url).ConfigureAwait(false);
        }
 
        public async Task<ListResponse<PaymentLinkResponse>> GetPaymentLinkListAsync(string from = null, int? limit = null, string profileId = null, bool testmode = false)
        {
            if (!string.IsNullOrWhiteSpace(profileId) || testmode)
            {
                this.ValidateApiKeyIsOauthAccesstoken();
            }

            var queryParameters = this.BuildQueryParameters(
               profileId: profileId,
               testmode: testmode);

            return await this.GetListAsync<ListResponse<PaymentLinkResponse>>("payment-links", from, limit, queryParameters).ConfigureAwait(false);
        }

        private Dictionary<string, string> BuildQueryParameters(string profileId = null, bool testmode = false)
        {
            var result = new Dictionary<string, string>();
            result.AddValueIfTrue(nameof(testmode), testmode);
            result.AddValueIfNotNullOrEmpty(nameof(profileId), profileId);
            return result;
        }
    }

    public interface IPaymentLinkClient {
        /*
            https://docs.mollie.com/reference/v2/payment-links-api/create-payment-link
            https://docs.mollie.com/reference/v2/payment-links-api/list-payment-links
            https://docs.mollie.com/reference/v2/payment-links-api/get-payment-link
         */
        Task<PaymentLinkResponse> CreatePaymentLinkAsync(PaymentLinkRequest paymentLinkRequest);

        /// <summary>
        ///	Retrieve a single payment link object by its token.
        /// </summary>
        /// <param name="paymentLinkId">The payment link's ID, for example pl_4Y0eZitmBnQ6IDoMqZQKh.</param>
        ///// <param name="testmode">Oauth - Optional – Set this to true to get a payment link made in test mode. If you omit this parameter, you can only retrieve live mode paymentLinks.</param>
        /// <returns></returns>
        Task<PaymentLinkResponse> GetPaymentLinkAsync(string paymentLinkId, bool testmode = false);
         
        /// <summary>
        /// Retrieve all payment links created with the current payment link profile, ordered from newest to oldest.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
		Task<ListResponse<PaymentLinkResponse>> GetPaymentLinkListAsync(string from = null, int? limit = null, string profileId = null, bool testmode = false);
        Task<ListResponse<PaymentLinkResponse>> GetPaymentLinkListAsync(UrlObjectLink<ListResponse<PaymentLinkResponse>> url);
        Task<PaymentLinkResponse> GetPaymentLinkAsync(UrlObjectLink<PaymentLinkResponse> url); 
    }
}