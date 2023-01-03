using System.Net.Http;
using System.Threading.Tasks;
using matcrm.data.Models.MollieModel.Customer;
using matcrm.data.Models.MollieModel.List;
using matcrm.data.Models.MollieModel.Payment.Request;
using matcrm.data.Models.MollieModel.Payment.Response;
using matcrm.data.Models.MollieModel.Url;
using matcrm.service.Services.Client;

namespace matcrm.service.Services.Mollie.Client {
    public class CustomerClient : BaseMollieClient, ICustomerClient {
        public CustomerClient(string apiKey, HttpClient httpClient = null) : base(apiKey, httpClient) {
        }

        public async Task<CustomerResponse> CreateCustomerAsync(CustomerRequest request) {
            return await this.PostAsync<CustomerResponse>($"customers", request).ConfigureAwait(false);
        }

        public async Task<CustomerResponse> UpdateCustomerAsync(string customerId, CustomerRequest request) {
            return await this.PostAsync<CustomerResponse>($"customers/{customerId}", request).ConfigureAwait(false);
        }

        public async Task DeleteCustomerAsync(string customerId) {
            await this.DeleteAsync($"customers/{customerId}").ConfigureAwait(false);
        }

        public async Task<CustomerResponse> GetCustomerAsync(string customerId) {
            return await this.GetAsync<CustomerResponse>($"customers/{customerId}").ConfigureAwait(false);
        }

        public async Task<CustomerResponse> GetCustomerAsync(UrlObjectLink<CustomerResponse> url) {
            return await this.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<ListResponse<CustomerResponse>> GetCustomerListAsync(UrlObjectLink<ListResponse<CustomerResponse>> url) {
            return await this.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<ListResponse<CustomerResponse>> GetCustomerListAsync(string from = null, int? limit = null) {
            return await this.GetListAsync<ListResponse<CustomerResponse>>("customers", from, limit)
                .ConfigureAwait(false);
        }

        public async Task<ListResponse<PaymentResponse>> GetCustomerPaymentListAsync(string customerId, string from = null, int? limit = null) {
            return await this.GetListAsync<ListResponse<PaymentResponse>>($"customers/{customerId}/payments", from, limit).ConfigureAwait(false);
        }

        public async Task<PaymentResponse> CreateCustomerPayment(string customerId, PaymentRequest paymentRequest) {
            return await this.PostAsync<PaymentResponse>($"customers/{customerId}/payments", paymentRequest).ConfigureAwait(false);
        }
    }

    public interface ICustomerClient {
        Task<CustomerResponse> CreateCustomerAsync(CustomerRequest request);
        Task<CustomerResponse> UpdateCustomerAsync(string customerId, CustomerRequest request);
        Task DeleteCustomerAsync(string customerId);
        Task<CustomerResponse> GetCustomerAsync(string customerId);
        Task<CustomerResponse> GetCustomerAsync(UrlObjectLink<CustomerResponse> url);
        Task<ListResponse<CustomerResponse>> GetCustomerListAsync(UrlObjectLink<ListResponse<CustomerResponse>> url);
        Task<ListResponse<CustomerResponse>> GetCustomerListAsync(string from = null, int? limit = null);
        Task<ListResponse<PaymentResponse>> GetCustomerPaymentListAsync(string customerId, string from = null, int? limit = null);
        Task<PaymentResponse> CreateCustomerPayment(string customerId, PaymentRequest paymentRequest);
    }
}