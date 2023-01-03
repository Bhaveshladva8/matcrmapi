using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class EmployeeTaskTimeRecordDetailResponse
    {
        public long Id { get; set; }
        public long? Duration { get; set; }
        public string? Comment { get; set; }
        public bool? IsBillable { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}