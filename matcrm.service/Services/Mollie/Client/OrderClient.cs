using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using matcrm.data.Extensions;
using matcrm.data.Models.MollieModel.List;
using matcrm.data.Models.MollieModel.Order;
using matcrm.data.Models.MollieModel.Payment.Response;
using matcrm.data.Models.MollieModel.Refund;
using matcrm.data.Models.MollieModel.Url;
using matcrm.service.Services.Client;

namespace matcrm.service.Services.Mollie.Client {
    public class OrderClient : BaseMollieClient, IOrderClient {
        public OrderClient(string apiKey, HttpClient httpClient = null) : base(apiKey, httpClient) {
        }

        public async Task<OrderResponse> CreateOrderAsync(OrderRequest orderRequest) {
            return await this.PostAsync<OrderResponse>("orders", orderRequest).ConfigureAwait(false);
        }

        public async Task<OrderResponse> GetOrderAsync(string orderId, bool embedPayments = false, bool embedRefunds = false, bool embedShipments = false) {
            var embeds = this.BuildQueryParameters(embedPayments, embedRefunds, embedShipments);
            return await this.GetAsync<OrderResponse>($"orders/{orderId}{embeds.ToQueryString()}").ConfigureAwait(false);
        }

        public async Task<OrderResponse> UpdateOrderAsync(string orderId, OrderUpdateRequest orderUpdateRequest) {
            return await this.PatchAsync<OrderResponse>($"orders/{orderId}", orderUpdateRequest).ConfigureAwait(false); 
        }

        public async Task<OrderResponse> UpdateOrderLinesAsync(string orderId, string orderLineId, OrderLineUpdateRequest orderLineUpdateRequest) {
            return await this.PatchAsync<OrderResponse>($"orders/{orderId}/lines/{orderLineId}", orderLineUpdateRequest).ConfigureAwait(false);
        }

        public async Task CancelOrderAsync(string orderId) {
            await this.DeleteAsync($"orders/{orderId}").ConfigureAwait(false);
        }

        public async Task<ListResponse<OrderResponse>> GetOrderListAsync(string from = null, int? limit = null) {
            return await this.GetListAsync<ListResponse<OrderResponse>>($"orders", from, limit).ConfigureAwait(false);
        }

        public async Task<ListResponse<OrderResponse>> GetOrderListAsync(UrlObjectLink<ListResponse<OrderResponse>> url) {
            return await this.GetAsync(url).ConfigureAwait(false);
        }

        public async Task CancelOrderLinesAsync(string orderId, OrderLineCancellationRequest cancelationRequest) {
            await this.DeleteAsync($"orders/{orderId}/lines", cancelationRequest).ConfigureAwait(false);
        }

        public async Task<PaymentResponse> CreateOrderPaymentAsync(string orderId, OrderPaymentRequest createOrderPaymentRequest) {
            return await this.PostAsync<PaymentResponse>($"orders/{orderId}/payments", createOrderPaymentRequest).ConfigureAwait(false);
        }

        public async Task<OrderRefundResponse> CreateOrderRefundAsync(string orderId, OrderRefundRequest createOrderRefundRequest) {
            return await this.PostAsync<OrderRefundResponse>($"orders/{orderId}/refunds", createOrderRefundRequest);
        }

        public async Task<ListResponse<RefundResponse>> GetOrderRefundListAsync(string orderId, string from = null, int? limit = null) {
            return await this.GetListAsync<ListResponse<RefundResponse>>($"orders/{orderId}/refunds", from, limit).ConfigureAwait(false);
        }

        private Dictionary<string, string> BuildQueryParameters(bool embedPayments = false, bool embedRefunds = false, bool embedShipments = false) {
            var result = new Dictionary<string, string>();
            result.AddValueIfNotNullOrEmpty("embed", this.BuildEmbedParameter(embedPayments, embedRefunds, embedShipments));
            return result;
        }

        private string BuildEmbedParameter(bool embedPayments = false, bool embedRefunds = false, bool embedShipments = false) {
            var includeList = new List<string>();
            includeList.AddValueIfTrue("payments", embedPayments);
            includeList.AddValueIfTrue("refunds", embedRefunds);
            includeList.AddValueIfTrue("shipments", embedShipments);
            return includeList.ToIncludeParameter();
        }
    }

    public interface IOrderClient {
        Task<OrderResponse> CreateOrderAsync(OrderRequest orderRequest);
        Task<OrderResponse> GetOrderAsync(string orderId, bool embedPayments = false, bool embedRefunds = false, bool embedShipments = false);
        Task<OrderResponse> UpdateOrderAsync(string orderId, OrderUpdateRequest orderUpdateRequest);
        Task<OrderResponse> UpdateOrderLinesAsync(string orderId, string orderLineId, OrderLineUpdateRequest orderLineUpdateRequest);
        Task CancelOrderAsync(string orderId);
        Task<ListResponse<OrderResponse>> GetOrderListAsync(string from = null, int? limit = null);
        Task<ListResponse<OrderResponse>> GetOrderListAsync(UrlObjectLink<ListResponse<OrderResponse>> url);
        Task CancelOrderLinesAsync(string orderId, OrderLineCancellationRequest cancelationRequest);
        Task<PaymentResponse> CreateOrderPaymentAsync(string orderId, OrderPaymentRequest createOrderPaymentRequest);
        Task<OrderRefundResponse> CreateOrderRefundAsync(string orderId, OrderRefundRequest createOrderRefundRequest);
        Task<ListResponse<RefundResponse>> GetOrderRefundListAsync(string orderId, string from = null, int? limit = null);
    }
}