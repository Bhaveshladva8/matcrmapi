using System;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Response
{
    public class OrganizationAttachUpdateDescResponse
    {
        public long? Id { get; set; }
        public long? OrganizationId { get; set; }
        public string Description { get; set; }
        
    }
}