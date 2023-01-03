using System;

namespace matcrm.data.Models.Response
{
    public class WeClappUserAddUpdateResponse
    {
        public long? Id { get; set; }        
        public string TenantName { get; set; }        
        public string ApiKey { get; set; }
        public int? UserId { get; set; }
    }
}