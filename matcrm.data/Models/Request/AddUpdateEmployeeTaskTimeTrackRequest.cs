using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class AddUpdateEmployeeTaskTimeTrackRequest
    {
        public long? Duration { get; set; }
        public long? EmployeeTaskId { get; set; }
        public long? ServiceArticleId { get; set; }
        public string Comment { get; set; }
        public bool IsBillable { get; set; }
    }
}