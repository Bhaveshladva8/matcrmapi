using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class EmployeeSubTaskUserDto
    {
        public long? Id { get; set; }
        public int? UserId { get; set; }
        public string? Name { get; set; }
        public long? EmployeeSubTaskId { get; set; }
        public virtual EmployeeSubTask EmployeeSubTask { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}