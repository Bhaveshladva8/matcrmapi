using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Request
{
    public class TaxAddRequest
    {
        public TaxAddRequest()
        {
            TaxRates = new List<TaxRateAddRequest>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public List<TaxRateAddRequest> TaxRates { get; set; }        
    }
}