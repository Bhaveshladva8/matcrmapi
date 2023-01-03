using System;

namespace matcrm.data.Models.Dto {
    public class TenantDto {
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public bool IsAdmin { get; set; }
        public string ApiKey { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? BlockedOn { get; set; }
        public int? BlockedBy { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}