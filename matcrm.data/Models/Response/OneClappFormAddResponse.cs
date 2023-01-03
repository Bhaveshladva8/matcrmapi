using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class OneClappFormAddResponse
    {
        public long? Id { get; set; }
        public string? FormKey { get; set; }
        public string? Name { get; set; }
    }
}