using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class InvoicePaymentRequest
    {
        public long ClientId { get; set; }
        public string InvoiceNo { get; set; }
    }
}