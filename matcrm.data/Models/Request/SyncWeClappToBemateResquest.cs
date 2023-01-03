using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class SyncWeClappToBemateResquest
    {
        public SyncWeClappToBemateResquest()
        {
            ContactPropertyList = new List<ContactProperty>();
        }
        public int TenantId { get; set; }
        public int UserId { get; set; }
        public List<ContactProperty> ContactPropertyList { get; set; }
    }
}