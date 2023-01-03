using System;

namespace matcrm.data.Models.Request
{
    public class WeClappUserDeleteRequest
    {
        public long? Id { get; set; }        
        public string TenantName { get; set; }        
        public string ApiKey { get; set; }
        public int? UserId { get; set; }        
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        
    }
}