using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.ViewModels
{
    public class SalesInvoiceVM
    {
        public string? CustomerId { get; set; }
        public string? SalesorderId { get; set; }
        public string? customerNumber { get; set; }
        public string? invoiceNumber { get; set; }
        public long? invoiceDate { get; set; }
        public string? netAmount { get; set; }
        public string? paymentStatus { get; set; }
        public string? status { get; set; }
    }
}