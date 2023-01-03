using System;

namespace matcrm.data.Models.Response
{
    public class ERPSystemColumnMapAddUpdateResponse
    {
        public long? Id { get; set; }        
        public long? CustomModuleId { get; set; }
        public string DestinationColumnName { get; set; }        
        public string SourceColumnName { get; set; }
        public long? UserId { get; set; }
        public long? WeClappUserId { get; set; }
    }
}