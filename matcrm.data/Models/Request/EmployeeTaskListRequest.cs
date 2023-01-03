using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class EmployeeTaskListRequest
    {
        // public string SearchString { get; set; }
        // public int PageNumber { get; set; } = 1;
        // public int PageSize { get; set; } = 10;
        public string SearchString { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string ShortColumnName { get; set; }
        public string SortType { get; set; }
        public long? StatusId { get; set; }
        //public long? ProjectId { get; set; }
    }
}