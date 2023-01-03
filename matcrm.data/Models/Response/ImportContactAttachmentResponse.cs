using System;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Response
{
    public class ImportContactAttachmentResponse
    {
        public long? Id { get; set; }
        public string FileName { get; set; }
        public long? ModuleId { get; set; }
        public string ModuleName { get; set; }        
        public IFormFile File { get; set; }
        
    }
}