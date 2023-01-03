namespace matcrm.data.Models.Request
{
    public class ERPSystemColumnMapGetAllRequest
    {
        public string TableName { get; set; }
        public long? CustomModuleId { get; set; }
        //public long? UserId { get; set; }
    }
}