using System;

namespace matcrm.data.Models.Response
{
    public class OrganizationAttachUploadFilesResponse
    {        
        public long? Id { get; set; }
        public string FileName { get; set; }        
        public long? OrganizationId { get; set; }        
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}