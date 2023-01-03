using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ServiceArticlePriceAddResponse
    {
        public long Id { get; set; }
        public long? ClientId { get; set; }
        public long? ServiceArticleId { get; set; }
        public long? Price { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsBillable { get; set; }
    }
}