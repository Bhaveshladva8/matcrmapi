using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class TaxRateListResponse
    {
        public long Id { get; set; }
        public string TaxType { get; set; }
        public long? Percentage { get; set; }
        public int CreatedBy { get; set; }
    }
}