using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class TaxAddResponse
    {
        public TaxAddResponse()
        {
            TaxRates = new List<TaxRateAddResponse>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Percentage { get; set; }
        public int CreatedBy { get; set; }
        public List<TaxRateAddResponse> TaxRates { get; set; }
    }
}