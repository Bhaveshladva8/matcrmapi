using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class EmployeeChildTaskActivityDto
    {
        public long? Id { get; set; }
        public long? EmployeeChildTaskId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? ShortName { get; set; }
        public virtual EmployeeChildTask EmployeeChildTask { get; set; }
        public int? UserId { get; set; }
        public virtual User User { get; set; }
        public string? Activity { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}