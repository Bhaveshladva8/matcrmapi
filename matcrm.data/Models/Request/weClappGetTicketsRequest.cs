namespace matcrm.data.Models.Request
{
    public class weClappGetTicketsRequest
    {
        public string ApiKey { get; set; }
        public string CustomerId { get; set; }
        public string Tenant { get; set; }
        public int? TenantId { get; set; }
    }
}