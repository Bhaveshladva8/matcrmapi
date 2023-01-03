using System;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Dto
{
    public class ImportContactAttachmentDto
    {
        public long? Id { get; set; }
        public string FileName { get; set; }
        public long? ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int? TenantId { get; set; }
        public int? CreatedBy { get; set; }
        public int? UserId { get; set; }
        public IFormFile File { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}