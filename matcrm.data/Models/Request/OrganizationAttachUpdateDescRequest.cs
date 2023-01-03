using System;

namespace matcrm.data.Models.Request
{
    public class OrganizationAttachUpdateDescRequest
    {
        public long? Id { get; set; }        
        public long? OrganizationId { get; set; }        
        public string Description { get; set; }
        
    }
}