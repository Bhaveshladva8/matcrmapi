using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Response
{
    public class ContractListResponse
    {
        public long? Id { get; set; }
        public string ContractType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public string BillingPeriod { get; set; }
        public DateTime? NextInvoiceDate { get; set; }
        //public string Name { get; set; }
        //public string Description { get; set; }
        public long? ClientId { get; set; }
    }
}