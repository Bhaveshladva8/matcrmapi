using matcrm.data.Models.MollieModel.Onboarding.Request;
using matcrm.data.Models.MollieModel.Onboarding.Response;
using System.Net.Http;
using System.Threading.Tasks;
using matcrm.service.Services.Client;

namespace matcrm.service.Services.Mollie.Client {
    public class OnboardingClient : BaseMollieClient, IOnboardingClient {
        public OnboardingClient(string apiKey, HttpClient httpClient = null) : base(apiKey, httpClient) {
        }

        public async Task<OnboardingStatusResponse> GetOnboardingStatusAsync() {
            return await this.GetAsync<OnboardingStatusResponse>("onboarding/me").ConfigureAwait(false);
        }

        public async Task SubmitOnboardingDataAsync(SubmitOnboardingDataRequest request) {
            await this.PostAsync<object>("onboarding/me", request).ConfigureAwait(false);
        }
    }

    public interface IOnboardingClient {
        /// <summary>
        /// Get the status of onboarding of the authenticated organization.
        /// </summary>
        Task<OnboardingStatusResponse> GetOnboardingStatusAsync();

        /// <summary>
        /// Submit data that will be prefilled in the merchant’s onboarding. Please note that the data
        /// you submit will only be processed when the onboarding status is needs-data.
        /// </summary>
        Task SubmitOnboardingDataAsync(SubmitOnboardingDataRequest request);
    }
}
