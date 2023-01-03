using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ContractActivityHistoryResponse
    {
        public long Id { get; set; }
        public long? ContractId { get; set; }
        public long? ClientId { get; set; }
        public string Activity { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }        
        public string LastName { get; set; }
        
    }
}