using System;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Response
{
    public class CustomerAttachmentGetAllResposne
    {
        public CustomerAttachmentGetAllResposne(){
            FileList = new IFormFile[] {};
        }
        public long? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShortName { get; set; }
        public string FileName { get; set; }
        public IFormFile[] FileList { get; set; }        
        public string Email { get; set; }
        public long? CustomerId { get; set; }        
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}