using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ContractAssetDetailResponse
    {
        public long Id { get; set; }
        public long ContractId { get; set; }
        public long ManufacturerId { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? BuyDate { get; set; }
        public DateTime? ServiceExpireDate { get; set; }
    }
}