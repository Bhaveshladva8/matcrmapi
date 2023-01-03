using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class EmployeeSubTaskActivityDto
    {
        public long? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? ShortName { get; set; }
        public long? EmployeeSubTaskId { get; set; }
        public virtual EmployeeSubTask EmployeeSubTask { get; set; }
        public int? UserId { get; set; }
        public virtual User User { get; set; }
        public string? Activity { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}