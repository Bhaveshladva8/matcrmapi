namespace matcrm.data.Models.Request
{
    public class WeClappUserAddUpdateRequest
    {
        public long? Id { get; set; }        
        public string TenantName { get; set; }       
        public string ApiKey { get; set; }
        //public int? UserId { get; set; }
        
    }
}