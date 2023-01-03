using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class AddInvoiceRequest
    {
        public long ClientId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}