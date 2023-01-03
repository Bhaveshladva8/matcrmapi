using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class CurrencyAddRequest
    {
        public long? Id { get; set; }
        public string CountryName { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Symbol { get; set; }        
    }
}