using System;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Request
{
    public class OrganizationAttachmentDeleteRequest
    {        
        public long? Id { get; set; }
        public long? OrganizationId { get; set; }
    }
}