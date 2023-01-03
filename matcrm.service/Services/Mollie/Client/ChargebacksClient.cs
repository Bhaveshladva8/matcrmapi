using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using matcrm.data.Extensions;
using matcrm.data.Models.MollieModel.Chargeback;
using matcrm.data.Models.MollieModel.List;
using matcrm.service.Services.Client;

namespace matcrm.service.Services.Mollie.Client {
    public class ChargebacksClient : BaseMollieClient, IChargebacksClient {
        public ChargebacksClient(string apiKey, HttpClient httpClient = null) : base(apiKey, httpClient) {
        }

        public async Task<ChargebackResponse> GetChargebackAsync(string paymentId, string chargebackId) {
            return await this.GetAsync<ChargebackResponse>($"payments/{paymentId}/chargebacks/{chargebackId}")
                .ConfigureAwait(false);
        }

        public async Task<ListResponse<ChargebackResponse>> GetChargebacksListAsync(string paymentId, string from = null, int? limit = null) {
            return await this
                .GetListAsync<ListResponse<ChargebackResponse>>($"payments/{paymentId}/chargebacks", from, limit)
                .ConfigureAwait(false);
        }

        public async Task<ListResponse<ChargebackResponse>> GetChargebacksListAsync(string profileId = null, bool? testmode = null) {
            if (profileId != null || testmode != null) {
                this.ValidateApiKeyIsOauthAccesstoken();
            }

            // Build parameters
            var parameters = new Dictionary<string, string>();
            parameters.AddValueIfNotNullOrEmpty(nameof(profileId), profileId);
            parameters.AddValueIfNotNullOrEmpty(nameof(testmode), Convert.ToString(testmode).ToLower());

            return await this.GetListAsync<ListResponse<ChargebackResponse>>($"chargebacks", null, null, parameters).ConfigureAwait(false);
        }
    }
    public interface IChargebacksClient {
        Task<ChargebackResponse> GetChargebackAsync(string paymentId, string chargebackId);
        Task<ListResponse<ChargebackResponse>> GetChargebacksListAsync(string paymentId, string from = null, int? limit = null);
        Task<ListResponse<ChargebackResponse>> GetChargebacksListAsync(string profileId = null, bool? testmode = null);
    }
}