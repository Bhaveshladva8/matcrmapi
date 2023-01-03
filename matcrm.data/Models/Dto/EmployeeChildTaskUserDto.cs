using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class EmployeeChildTaskUserDto
    {
        public long? Id { get; set; }
        public int? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public long? EmployeeChildTaskId { get; set; }
        public virtual EmployeeChildTask EmployeeChildTask { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}