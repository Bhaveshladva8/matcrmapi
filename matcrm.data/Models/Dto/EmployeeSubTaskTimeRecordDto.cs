using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class EmployeeSubTaskTimeRecordDto
    {
        public long? Id { get; set; }
        public int? UserId { get; set; }
        public virtual User User { get; set; }
        public long? Duration { get; set; }
        public long? SubTaskId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}