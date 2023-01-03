using System;

namespace matcrm.data.Models.Dto
{
    public class ERPSystemColumnMapDto
    {
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public long? ERPId { get; set; }
        public long? WeClappUserId { get; set; }
        public string SourceColumnName { get; set; }
        public string DestinationColumnName { get; set; }
        public long? CustomModuleId { get; set; }
        public string TableName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}