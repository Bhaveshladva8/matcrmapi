using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class AddUpdateTaskTimeTrackeResponse
    {
        public long? Id { get; set; }
        public long? Duration { get; set; }
        public long? EmployeeTaskId { get; set; }
        public string ServiceArticleName { get; set; }
        public long? Price { get; set; }
        public string TotalAmount { get; set; }
    }
}