using System.Net.Http;
using System.Threading.Tasks;
using matcrm.data.Models.MollieModel.List;
using matcrm.data.Models.MollieModel.Shipment;
using matcrm.service.Services.Client;

namespace matcrm.service.Services.Mollie.Client
{
    public class ShipmentClient : BaseMollieClient, IShipmentClient {
        public ShipmentClient(string apiKey, HttpClient httpClient = null) : base(apiKey, httpClient) {
        }

        public async Task<ShipmentResponse> CreateShipmentAsync(string orderId, ShipmentRequest shipmentRequest) {
            return await this.PostAsync<ShipmentResponse>($"orders/{orderId}/shipments", shipmentRequest).ConfigureAwait(false);
        }

        public async Task<ShipmentResponse> GetShipmentAsync(string orderId, string shipmentId) {
            return await this.GetAsync<ShipmentResponse>($"orders/{orderId}/shipments/{shipmentId}").ConfigureAwait(false);
        }

        public async Task<ListResponse<ShipmentResponse>> GetShipmentsListAsync(string orderId) {
            return await this.GetAsync<ListResponse<ShipmentResponse>>($"orders/{orderId}/shipments").ConfigureAwait(false);
        }

        public async Task<ShipmentResponse> UpdateOrderAsync(string orderId, string shipmentId, ShipmentUpdateRequest shipmentUpdateRequest) {
            return await this.PatchAsync<ShipmentResponse>($"orders/{orderId}/shipments/{shipmentId}", shipmentUpdateRequest).ConfigureAwait(false); 
        }
    }

    public interface IShipmentClient {
        Task<ShipmentResponse> CreateShipmentAsync(string orderId, ShipmentRequest shipmentRequest);
        Task<ShipmentResponse> GetShipmentAsync(string orderId, string shipmentId);
        Task<ListResponse<ShipmentResponse>> GetShipmentsListAsync(string orderId);
        Task<ShipmentResponse> UpdateOrderAsync(string orderId, string shipmentId, ShipmentUpdateRequest shipmentUpdateRequest);
    }
}