using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class EmployeeTaskDropDownListRequest
    {
        public long? Id { get; set; }
        public string Type {get; set; }
    }
}