using System;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Request
{
    public class CustomerAttachmentUploadFilesRequest
    {
        public CustomerAttachmentUploadFilesRequest(){
            FileList = new IFormFile[] {};
        }
        public long? CustomerId { get; set; }        
        public IFormFile[] FileList { get; set; }
        

    }
}