using System.ComponentModel.DataAnnotations;

namespace matcrm.data.Models.ViewModels {
    public class TenantVM {
        public string Username { get; set; }
        public string ApiKey { get; set; }
        public string Tenant { get; set; }
        public int Id { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSuccess { get; set; }
        public string accessToken { get; set; }
        public string expires_in { get; set; }
    }
    public class TenantVMResult {
        public TenantVM Result { get; set; }
    }
}