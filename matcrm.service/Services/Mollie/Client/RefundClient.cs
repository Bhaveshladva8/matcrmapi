﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using matcrm.data.Extensions;
using matcrm.data.Models.MollieModel.List;
using matcrm.data.Models.MollieModel.Refund;
using matcrm.data.Models.MollieModel.Url;
using matcrm.service.Services.Client;

namespace matcrm.service.Services.Mollie.Client {
    public class RefundClient : BaseMollieClient, IRefundClient {
        public RefundClient(string apiKey, HttpClient httpClient = null) : base(apiKey, httpClient) {
        }

        public async Task<RefundResponse> CreateRefundAsync(string paymentId, RefundRequest refundRequest) {
            if (refundRequest.Testmode.HasValue)
            {
                this.ValidateApiKeyIsOauthAccesstoken();
            }

            return await this.PostAsync<RefundResponse>($"payments/{paymentId}/refunds", refundRequest).ConfigureAwait(false);
        }

        public async Task<ListResponse<RefundResponse>> GetRefundListAsync(string from = null, int? limit = null, bool testmode = false) {
            var queryParameters = this.BuildQueryParameters(testmode: testmode);
            
            return await this.GetListAsync<ListResponse<RefundResponse>>($"refunds", from, limit, queryParameters).ConfigureAwait(false);
        }
        
        public async Task<ListResponse<RefundResponse>> GetRefundListAsync(string paymentId, string from = null, int? limit = null, bool testmode = false) {
            var queryParameters = this.BuildQueryParameters(testmode: testmode);

            return await this.GetListAsync<ListResponse<RefundResponse>>($"payments/{paymentId}/refunds", from, limit, queryParameters).ConfigureAwait(false);
        }

        public async Task<RefundResponse> GetRefundAsync(UrlObjectLink<RefundResponse> url) {
            return await this.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<RefundResponse> GetRefundAsync(string paymentId, string refundId, bool testmode = false) {
            var queryParameters = this.BuildQueryParameters(testmode: testmode);
            return await this.GetAsync<RefundResponse>($"payments/{paymentId}/refunds/{refundId}{queryParameters.ToQueryString()}").ConfigureAwait(false);
        }

        public async Task CancelRefundAsync(string paymentId, string refundId, bool testmode = default) {
            var queryParameters = this.BuildQueryParameters(testmode: testmode);
            await this.DeleteAsync($"payments/{paymentId}/refunds/{refundId}{queryParameters.ToQueryString()}").ConfigureAwait(false);
        }
        
        private Dictionary<string, string> BuildQueryParameters(bool testmode = false) {
            var result = new Dictionary<string, string>();
            result.AddValueIfTrue(nameof(testmode), testmode);
            return result;
        }
    }

    public interface IRefundClient {
        Task CancelRefundAsync(string paymentId, string refundId, bool testmode = false);
        Task<RefundResponse> CreateRefundAsync(string paymentId, RefundRequest refundRequest);
        Task<RefundResponse> GetRefundAsync(string paymentId, string refundId, bool testmode = false);
        Task<ListResponse<RefundResponse>> GetRefundListAsync(string paymentId, string from = null, int? limit = null, bool testmode = false);
        Task<RefundResponse> GetRefundAsync(UrlObjectLink<RefundResponse> url);
    }
}