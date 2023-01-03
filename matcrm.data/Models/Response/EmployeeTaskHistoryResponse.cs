using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class EmployeeTaskHistoryResponse
    {
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public string ProfileURL { get; set; }
        public string Activity { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}