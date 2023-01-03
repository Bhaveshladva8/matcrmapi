using matcrm.data.Models.MollieModel.Url;

namespace matcrm.data.Models.MollieModel.Permission {
    public class PermissionResponseLinks {
        /// <summary>
        /// The API resource URL of the permission itself.
        /// </summary>
        public UrlObjectLink<PermissionResponse> Self { get; set; }

        /// <summary>
        /// The URL to the permission retrieval endpoint documentation.
        /// </summary>
        public UrlLink Documentation { get; set; }
    }
}