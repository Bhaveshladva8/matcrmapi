using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Dto
{
    public class SyncContactDto
    {
        public SyncContactDto()
        {
            ContactPropertyList = new List<ContactProperty>();
        }
        public int TenantId { get; set; }
        public int UserId { get; set; }
        public List<ContactProperty> ContactPropertyList { get; set; }
    }

      public class ContactProperty
    {
        public List<ContactKeyValue> ContactProperties { get; set; }
    }

    public class ContactKeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}