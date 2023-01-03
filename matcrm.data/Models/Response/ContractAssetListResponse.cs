using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ContractAssetListResponse
    {
        public long Id { get; set; }
        public string SerialNumber { get; set; }
        public string Manufacturer { get; set; }
        public DateTime? BuyDate { get; set; }
        public DateTime? ServiceExpireDate { get; set; }
    }
}