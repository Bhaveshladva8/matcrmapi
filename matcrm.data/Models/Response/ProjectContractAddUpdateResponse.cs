using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ProjectContractAddUpdateResponse
    {
        public long Id { get; set; }
        public long? ProjectId { get; set; }
        public long? ContractId { get; set; }
    }
}