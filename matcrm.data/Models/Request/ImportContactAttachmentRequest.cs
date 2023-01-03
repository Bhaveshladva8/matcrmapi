using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Request
{
    public class ImportContactAttachmentRequest
    {
        
        public string ModuleName { get; set; }        
        public IFormFile File { get; set; }
        
    }
}