using System;

namespace matcrm.data.Models.Response
{
    public class CustomerAttachmentUploadFilesResponse
    {
        public long? Id { get; set; }
        public string FileName { get; set; }
        public long? CustomerId { get; set; }        
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}