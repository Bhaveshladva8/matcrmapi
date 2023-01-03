using System.Net.Http;
using System.Threading.Tasks;
using matcrm.data.Models.MollieModel.Capture;
using matcrm.data.Models.MollieModel.List;
using matcrm.service.Services.Client;

namespace matcrm.service.Services.Mollie.Client
{
    public class CaptureClient : BaseMollieClient, ICaptureClient {
        public CaptureClient(string apiKey, HttpClient httpClient = null) : base(apiKey, httpClient) {
        }

        public async Task<CaptureResponse> GetCaptureAsync(string paymentId, string captureId) {
            return await this.GetAsync<CaptureResponse>($"payments/{paymentId}/captures/{captureId}").ConfigureAwait(false);
        }

        public async Task<ListResponse<CaptureResponse>> GetCapturesListAsync(string paymentId) {
            return await this.GetAsync<ListResponse<CaptureResponse>>($"payments/{paymentId}/captures").ConfigureAwait(false);
        }
    }

    public interface ICaptureClient {
        Task<CaptureResponse> GetCaptureAsync(string paymentId, string captureId);
        Task<ListResponse<CaptureResponse>> GetCapturesListAsync(string paymentId);
    }
}