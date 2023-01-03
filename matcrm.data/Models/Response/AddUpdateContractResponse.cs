using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class AddUpdateContractResponse
    {
        // public AddUpdateContractResponse(){
        //     Subscriptions = new List<ContractServiceArticleResponse>();
        // }
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
        public string CurrencyName { get; set; }
        //public List<ContractServiceArticleResponse> Subscriptions { get; set; }
    }
}