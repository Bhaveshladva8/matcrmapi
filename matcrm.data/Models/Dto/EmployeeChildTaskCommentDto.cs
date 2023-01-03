using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class EmployeeChildTaskCommentDto
    {
        public long? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public long? EmployeeChildTaskId { get; set; }
        public virtual EmployeeChildTask EmployeeChildTask { get; set; }
        public int? UserId { get; set; }
        public virtual User User { get; set; }
        public string? Name { get; set; }
        public string? Comment { get; set; }
    }
}