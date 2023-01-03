using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ContractServiceArticleResponse
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? UnitPrice { get; set; }
        public long? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
    }
}