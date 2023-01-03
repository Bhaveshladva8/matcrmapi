using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class OrganizationActivityDeleteRequest
    {       
        public long? Id { get; set; }
        public long? OrganizationId { get; set; }
        public int? CreatedBy { get; set; }        
        
    }
}