using System.Net.Http;
using System.Threading.Tasks;
using matcrm.data.Models.MollieModel.List;
using matcrm.data.Models.MollieModel.Organization;
using matcrm.data.Models.MollieModel.Url;

namespace matcrm.service.Services.Mollie.Client {
    public class OrganizationsClient : OauthBaseMollieClient, IOrganizationsClient {
        public OrganizationsClient(string oauthAccessToken, HttpClient httpClient = null) : base(oauthAccessToken, httpClient) {
        }

        public async Task<OrganizationResponse> GetCurrentOrganizationAsync() {
            return await this.GetAsync<OrganizationResponse>($"organizations/me").ConfigureAwait(false);
        }

        public async Task<OrganizationResponse> GetOrganizationAsync(string organizationId) {
            return await this.GetAsync<OrganizationResponse>($"organizations/{organizationId}").ConfigureAwait(false);
        }

        public async Task<ListResponse<OrganizationResponse>> GetOrganizationsListAsync(string from = null, int? limit = null) {
            return await this.GetListAsync<ListResponse<OrganizationResponse>>("organizations", from, limit, null).ConfigureAwait(false);
        }

        public async Task<OrganizationResponse> GetOrganizationAsync(UrlObjectLink<OrganizationResponse> url) {
            return await this.GetAsync(url).ConfigureAwait(false);
        }
    }
    public interface IOrganizationsClient
    {
        Task<OrganizationResponse> GetCurrentOrganizationAsync();
        Task<OrganizationResponse> GetOrganizationAsync(string organizationId);
        Task<ListResponse<OrganizationResponse>> GetOrganizationsListAsync(string from = null, int? limit = null);
        Task<OrganizationResponse> GetOrganizationAsync(UrlObjectLink<OrganizationResponse> url);
    }
}