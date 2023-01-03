using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class AddUpdateContractRequest
    {
        public AddUpdateContractRequest()
        {
            ServiceArticles = new List<ServiceArticleIdsAddContractRequest>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public long? Discount { get; set; }
        public long? ClientId { get; set; }
        public long? ContractTypeId { get; set; }
        public long? Amount { get; set; }
        public long? CurrencyId { get; set; }
        public bool IsBillingFromStartDate { get; set; }
        public long? DefaultUnitPrice { get; set; }        
        public long? InvoiceIntervalId { get; set; }
        public long? Interval { get; set; }
        public long? StatusId { get; set; }
        public long? AllowedHours { get; set; }
        public List<ServiceArticleIdsAddContractRequest> ServiceArticles { get; set; }
        // public List<long> ServiceArticleIds { get; set; }
        // public bool IsContractUnitPrice { get; set; }
    }

    public class ServiceArticleIdsAddContractRequest
    {
        public long? Id { get; set; }
        public long ServiceArticleId { get; set; }
        public bool IsContractUnitPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsBillable { get; set; }
    }

}