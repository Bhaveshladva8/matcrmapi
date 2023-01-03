using System.Net.Http;
using System.Threading.Tasks;
using matcrm.data.Models.MollieModel;
using matcrm.data.Models.MollieModel.List;

using matcrm.data.Models.MollieModel.Permission;
using matcrm.data.Models.MollieModel.Url;

namespace matcrm.service.Services.Mollie.Client {
    public class PermissionsClient : OauthBaseMollieClient, IPermissionsClient {
        public PermissionsClient(string oauthAccessToken, HttpClient httpClient = null) : base(oauthAccessToken, httpClient) {
        }

        public async Task<PermissionResponse> GetPermissionAsync(string permissionId) {
            return await this.GetAsync<PermissionResponse>($"permissions/{permissionId}").ConfigureAwait(false);
        }

        public async Task<PermissionResponse> GetPermissionAsync(UrlObjectLink<PermissionResponse> url) {
            return await this.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<ListResponse<PermissionResponse>> GetPermissionListAsync() {
            return await this.GetListAsync<ListResponse<PermissionResponse>>("permissions", null, null).ConfigureAwait(false);
        }
    }

    public interface IPermissionsClient {
        Task<PermissionResponse> GetPermissionAsync(string permissionId);
        Task<PermissionResponse> GetPermissionAsync(UrlObjectLink<PermissionResponse> url);
        Task<ListResponse<PermissionResponse>> GetPermissionListAsync();
    }
}