using System.Net.Http;
using System.Threading.Tasks;
using matcrm.data.Models.MollieModel.List;
using matcrm.data.Models.MollieModel.PaymentMethod;
using matcrm.data.Models.MollieModel.Profile.Request;
using matcrm.data.Models.MollieModel.Profile.Response;
using matcrm.service.Services.Client;

namespace matcrm.service.Services.Mollie.Client {
    public class ProfileClient : BaseMollieClient, IProfileClient {
        public ProfileClient(string apiKey, HttpClient httpClient = null) : base(apiKey, httpClient) {
        }

        public async Task<ProfileResponse> CreateProfileAsync(ProfileRequest request) {
            return await this.PostAsync<ProfileResponse>("profiles", request).ConfigureAwait(false);
        }

        public async Task<ProfileResponse> GetProfileAsync(string profileId) {
            return await this.GetAsync<ProfileResponse>($"profiles/{profileId}").ConfigureAwait(false);
        }

        public async Task<ProfileResponse> GetCurrentProfileAsync() {
            return await this.GetAsync<ProfileResponse>("profiles/me").ConfigureAwait(false);
        }

        public async Task<ListResponse<ProfileResponse>> GetProfileListAsync(string from = null, int? limit = null) {
            return await this.GetListAsync<ListResponse<ProfileResponse>>("profiles", from, limit).ConfigureAwait(false);
        }

        public async Task<ProfileResponse> UpdateProfileAsync(string profileId, ProfileRequest request) {
            return await this.PostAsync<ProfileResponse>($"profiles/{profileId}", request).ConfigureAwait(false);
        }

        public async Task<PaymentMethodResponse> EnablePaymentMethodAsync(string profileId, string paymentMethod) {
            return await this.PostAsync<PaymentMethodResponse>($"profiles/{profileId}/methods/{paymentMethod}", null).ConfigureAwait(false);
        }

        public async Task<PaymentMethodResponse> EnablePaymentMethodAsync(string paymentMethod) {
            return await this.PostAsync<PaymentMethodResponse>($"profiles/me/methods/{paymentMethod}", null).ConfigureAwait(false);
        }

        public async Task DisablePaymentMethodAsync(string profileId, string paymentMethod) {
            await this.DeleteAsync($"profiles/{profileId}/methods/{paymentMethod}").ConfigureAwait(false);
        }

        public async Task DisablePaymentMethodAsync(string paymentMethod) {
            await this.DeleteAsync($"profiles/me/methods/{paymentMethod}").ConfigureAwait(false);
        }

        public async Task DeleteProfileAsync(string profileId) {
            await this.DeleteAsync($"profiles/{profileId}").ConfigureAwait(false);
        }

        public async Task<EnableGiftCardIssuerResponse> EnableGiftCardIssuerAsync(string profileId, string issuer) {
            return await this.PostAsync<EnableGiftCardIssuerResponse>($"profiles/{profileId}/methods/giftcard/issuers/{issuer}", null).ConfigureAwait(false);
        }

        public async Task<EnableGiftCardIssuerResponse> EnableGiftCardIssuerAsync(string issuer) {
            return await this.PostAsync<EnableGiftCardIssuerResponse>($"profiles/me/methods/giftcard/issuers/{issuer}", null).ConfigureAwait(false);
        }

        public async Task DisableGiftCardIssuerAsync(string profileId, string issuer) {
            await this.DeleteAsync($"profiles/{profileId}/methods/giftcard/issuers/{issuer}", null).ConfigureAwait(false);
        }

        public async Task DisableGiftCardIssuerAsync(string issuer) {
            await this.DeleteAsync($"profiles/me/methods/giftcard/issuers/{issuer}", null).ConfigureAwait(false);
        }
    }

    public interface IProfileClient {
        Task<ProfileResponse> CreateProfileAsync(ProfileRequest request);
        Task<ProfileResponse> GetProfileAsync(string profileId);
        Task<ListResponse<ProfileResponse>> GetProfileListAsync(string from = null, int? limit = null);
        Task<ProfileResponse> UpdateProfileAsync(string profileId, ProfileRequest request);
        Task DeleteProfileAsync(string profileId);
        Task<ProfileResponse> GetCurrentProfileAsync();
        Task<PaymentMethodResponse> EnablePaymentMethodAsync(string profileId, string paymentMethod);
        Task<PaymentMethodResponse> EnablePaymentMethodAsync(string paymentMethod);
        Task DisablePaymentMethodAsync(string profileId, string paymentMethod);
        Task DisablePaymentMethodAsync(string paymentMethod);
        Task<EnableGiftCardIssuerResponse> EnableGiftCardIssuerAsync(string profileId, string issuer);
        Task<EnableGiftCardIssuerResponse> EnableGiftCardIssuerAsync(string issuer);
        Task DisableGiftCardIssuerAsync(string profileId, string issuer);
        Task DisableGiftCardIssuerAsync(string issuer);
    }
}