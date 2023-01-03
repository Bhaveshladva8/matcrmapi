namespace matcrm.data.Models.Request
{
    public class CustomModuleGetCustomFieldRequest
    {      
       
        public string TableName { get; set; }        
        public long? TenantId { get; set; }
        
    }
}