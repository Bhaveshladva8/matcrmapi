using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class ServiceArticleAddRequest
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? CategoryId { get; set; }
        public long? UnitPrice { get; set; }
        public long? CurrencyId { get; set; }        
        public bool IsTaxable { get; set; }
        public long? TaxId { get; set; }
    }
}