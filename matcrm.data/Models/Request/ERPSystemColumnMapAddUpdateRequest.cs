namespace matcrm.data.Models.Request
{
    public class ERPSystemColumnMapAddUpdateRequest
    {
        public long? Id { get; set; }
        public string DestinationColumnName { get; set; }
        public string SourceColumnName { get; set; }
        public string TableName { get; set; }
        public long? CustomModuleId { get; set; }
        public long? UserId { get; set; }
        public long? WeClappUserId { get; set; }
    }
}