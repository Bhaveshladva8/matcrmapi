using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class TaxDetailResponse
    {
        public TaxDetailResponse()
        {
            taxRates = new List<TaxRateDetailResponse>();
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public List<TaxRateDetailResponse> taxRates { get; set; }
    }
}