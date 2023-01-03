using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class TaxListResponse
    {
        public TaxListResponse()
        {
            TaxRates = new List<TaxRateListResponse>();
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Percentage { get; set; }
        public int CreatedBy { get; set; }
        public List<TaxRateListResponse> TaxRates { get; set; }
    }
}