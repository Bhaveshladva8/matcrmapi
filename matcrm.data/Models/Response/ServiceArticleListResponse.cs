using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ServiceArticleListResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        // public long? CategoryId { get; set; }
        // public string CategoryName { get; set; }
        public long CurrencyId { get; set; }
        public string UnitPrice { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}