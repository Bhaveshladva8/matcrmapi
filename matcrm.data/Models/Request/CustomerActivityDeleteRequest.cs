using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class CustomerActivityDeleteRequest
    {
        
        public long? Id { get; set; }
        public long? CustomerId { get; set; }        
        public int? CreatedBy { get; set; }
        
    }
}