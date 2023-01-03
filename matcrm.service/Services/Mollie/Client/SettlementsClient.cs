using System.Net.Http;
using System.Threading.Tasks;
using matcrm.data.Models.MollieModel.Capture;
using matcrm.data.Models.MollieModel.Chargeback;
using matcrm.data.Models.MollieModel.List;
using matcrm.data.Models.MollieModel.Payment.Response;
using matcrm.data.Models.MollieModel.Refund;
using matcrm.data.Models.MollieModel.Settlement;
using matcrm.data.Models.MollieModel.Url;
using matcrm.service.Services.Client;

namespace matcrm.service.Services.Mollie.Client {
    public class SettlementsClient : BaseMollieClient, ISettlementsClient {
        public SettlementsClient(string oauthAccessToken, HttpClient httpClient = null) : base(oauthAccessToken, httpClient) {
        }

        public async Task<SettlementResponse> GetSettlementAsync(string settlementId) {
            return await this.GetAsync<SettlementResponse>($"settlements/{settlementId}").ConfigureAwait(false);
        }

        public async Task<SettlementResponse> GetNextSettlement() {
            return await this.GetAsync<SettlementResponse>($"settlements/next").ConfigureAwait(false);
        }

        public async Task<SettlementResponse> GetOpenSettlement() {
            return await this.GetAsync<SettlementResponse>($"settlements/open").ConfigureAwait(false);
        }

        public async Task<ListResponse<SettlementResponse>> GetSettlementsListAsync(string reference = null, string offset = null, int? count = null) {
            var queryString = !string.IsNullOrWhiteSpace(reference) ? $"?reference={reference}" : string.Empty;
            return await this.GetListAsync<ListResponse<SettlementResponse>>($"settlements{queryString}", offset, count).ConfigureAwait(false);
        }

        public async Task<ListResponse<PaymentResponse>> GetSettlementPaymentsListAsync(string settlementId, string offset = null, int? count = null) {
            return await this.GetListAsync<ListResponse<PaymentResponse>>($"settlements/{settlementId}/payments", offset, count).ConfigureAwait(false);
        }

        public async Task<ListResponse<RefundResponse>> GetSettlementRefundsListAsync(string settlementId, string offset = null, int? count = null) {
            return await this.GetListAsync<ListResponse<RefundResponse>>($"settlements/{settlementId}/refunds", offset, count).ConfigureAwait(false);
        }

        public async Task<ListResponse<ChargebackResponse>> GetSettlementChargebacksListAsync(string settlementId, string offset = null, int? count = null) {
            return await this.GetListAsync<ListResponse<ChargebackResponse>>($"settlements/{settlementId}/chargebacks", offset, count).ConfigureAwait(false);
        }

        public async Task<SettlementResponse> GetSettlementAsync(UrlObjectLink<SettlementResponse> url) {
            return await this.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<ListResponse<CaptureResponse>> ListSettlementCapturesAsync(string settlementId) {
            return await this.GetListAsync<ListResponse<CaptureResponse>>($"settlements/{settlementId}/captures", null, null).ConfigureAwait(false);
        }
    }

    public interface ISettlementsClient {
        Task<SettlementResponse> GetSettlementAsync(string settlementId);
        Task<SettlementResponse> GetNextSettlement();
        Task<SettlementResponse> GetOpenSettlement();
        Task<ListResponse<SettlementResponse>> GetSettlementsListAsync(string reference = null, string from = null, int? limit = null);
        Task<ListResponse<PaymentResponse>> GetSettlementPaymentsListAsync(string settlementId, string from = null, int? limit = null);
        Task<ListResponse<RefundResponse>> GetSettlementRefundsListAsync(string settlementId, string from = null, int? limit = null);
        Task<ListResponse<ChargebackResponse>> GetSettlementChargebacksListAsync(string settlementId, string from = null, int? limit = null);
        Task<SettlementResponse> GetSettlementAsync(UrlObjectLink<SettlementResponse> url);
        Task<ListResponse<CaptureResponse>> ListSettlementCapturesAsync(string settlementId);
    }
}