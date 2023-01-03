using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class CommonListRequest
    {
        public string SearchString { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string ShortColumnName { get; set; }
        public string SortType { get; set; }
        public int? MainId { get; set; }
    }
}