using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Dto
{
    public class StatusDto
    {
        public StatusDto(){            
            CustomTables = new List<string>();
            CustomTableIds = new List<long>();
        }
        public long? Id { get; set; }
        public string? Name { get; set; }
         public string? Color { get; set; }
        public bool IsDefault { get; set; }
        public long? CustomTableId { get; set; }        
        public string CustomTableName { get; set; }
        public List<long> CustomTableIds { get; set; }   
         public List<string> CustomTables { get; set; }
        public int? TenantId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}