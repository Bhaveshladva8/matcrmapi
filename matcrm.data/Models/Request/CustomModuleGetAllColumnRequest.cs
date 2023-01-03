namespace matcrm.data.Models.Request
{
    public class CustomModuleGetAllColumnRequest
    {
        public string TableName { get; set; }
        public int? TenantId { get; set; }
        public int? UserId { get; set; }
    }
}