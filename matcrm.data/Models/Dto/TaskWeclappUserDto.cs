using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class TaskWeclappUserDto
    {
        public long? Id { get; set; }
        public string? TenantName { get; set; }
        [Column(TypeName = "varchar(1500)")]
        public string? ApiKey { get; set; }
        public int? UserId { get; set; }
    
        public int? TenantId { get; set; }
    
    }
}