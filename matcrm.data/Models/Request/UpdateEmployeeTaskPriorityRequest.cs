using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class UpdateEmployeeTaskPriorityRequest
    {
        public long? Id { get; set; }
        public long? Priority { get; set; }
        public long? CurrentPriority { get; set; }
        public long? PreviousPriority { get; set; }
        public long? CurrentSectionId { get; set; }
        public long? PreviousSectionId { get; set; }
        public bool IsSectionChange { get; set; }
    }
}