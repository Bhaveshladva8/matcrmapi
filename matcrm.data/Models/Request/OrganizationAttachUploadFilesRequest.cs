using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Request
{
    public class OrganizationAttachUploadFilesRequest
    {
        public OrganizationAttachUploadFilesRequest(){
            FileList = new IFormFile[] {};
        }
        public long? OrganizationId { get; set; }        
        public IFormFile[] FileList { get; set; }
    }
}