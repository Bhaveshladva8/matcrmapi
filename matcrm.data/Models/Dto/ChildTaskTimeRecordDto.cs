using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Dto
{
    public class ChildTaskTimeRecordDto
    {
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public long? Duration { get; set; }
        public long? ChildTaskId { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}