using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class LeadActivityDeleteRequest
    {        
        public long? Id { get; set; }       
        public long? LeadId { get; set; }        
        public int? CreatedBy { get; set; }
        
        
    }
}