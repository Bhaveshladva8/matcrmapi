using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class ClientPhoneAddUpdateRequest
    {
        public long? Id { get; set; }
        public string PhoneNo { get; set; }
        public long? PhoneNoTypeId { get; set; }
        public bool IsPrimary { get; set; }
        //public bool IsDelete { get; set; }  
    }
}