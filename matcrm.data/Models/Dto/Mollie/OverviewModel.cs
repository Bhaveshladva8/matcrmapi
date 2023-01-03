using System.Collections.Generic;
using matcrm.data.Models.MollieModel;

namespace matcrm.data.Models.Dto.Mollie {
    public class OverviewModel<T> where T : IResponseObject {
        public List<T> Items { get; set; }
        public OverviewNavigationLinksModel Navigation { get; set; }
    }
}