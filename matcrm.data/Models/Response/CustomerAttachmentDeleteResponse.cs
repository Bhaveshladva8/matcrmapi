using System;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Response
{
    public class CustomerAttachmentDeleteResponse
    {
        public CustomerAttachmentDeleteResponse(){
            FileList = new IFormFile[] {};
        }
        public long? Id { get; set; }        
        public IFormFile[] FileList { get; set; }        
        public long? CustomerId { get; set; }
        
    }
}