using System;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Request
{
    public class CustomerAttachDescriptionRequest
    {
       
        public long? Id { get; set; }        
        public string Description { get; set; }
        public long? CustomerId { get; set; }
        
    }
}