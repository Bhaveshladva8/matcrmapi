using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class AddUpdateInvoiceIntervalRequestResponse
    {
        public long? Id { get; set; }
        public string? Name { get; set; }
        public long? Interval { get; set; }

    }
}