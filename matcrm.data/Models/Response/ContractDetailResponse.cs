using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ContractDetailResponse
    {
        public ContractDetailResponse()
        {
            ServiceArticles = new List<ContractDetailServiceArticleResponse>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }       
        public long? Discount { get; set; }
        public long? ClientId { get; set; }
        public long? StatusId { get; set; }
        public string StatusName { get; set; }
        public long? InvoiceIntervalId { get; set; }
        public string InvoiceInterval { get; set; }
        public long? Interval { get; set; }
        public long? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public long? ContractTypeId { get; set; }
        public string ContractTypeName { get; set; }
        public long? AllowedHours { get; set; }
        //public string CurrencyCode { get; set; }
        public List<ContractDetailServiceArticleResponse> ServiceArticles { get; set; }
    }

    public class ContractDetailServiceArticleResponse
    {
        public long? Id { get; set; }
        public long? ServiceArticleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsBillable { get; set; }
        public bool IsContractUnitPrice { get; set; }
    }
}