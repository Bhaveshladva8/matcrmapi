using System;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Request
{
    public class CustomerAttachmentDeleteRequest
    {
        
        public long? Id { get; set; }        
        public long? CustomerId { get; set; }
        
    }
}