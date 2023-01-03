using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class SyncCustomerRequest
    {
        public SyncCustomerRequest()
        {
            ContactPropertyList = new List<ContactProperty>();
        }
        public int TenantId { get; set; }
        public int UserId { get; set; }
        public List<ContactProperty> ContactPropertyList { get; set; }
    }
}