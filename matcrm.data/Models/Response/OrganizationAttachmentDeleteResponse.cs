using System;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Response
{
    public class OrganizationAttachmentDeleteResponse
    {
        public OrganizationAttachmentDeleteResponse(){
            FileList = new IFormFile[] {};
        }
        public long? Id { get; set; }
        public IFormFile[] FileList { get; set; }        
        public long? OrganizationId { get; set; }
        
    }
}