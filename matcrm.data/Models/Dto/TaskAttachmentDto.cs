using System;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Dto {
    public class TaskAttachmentDto {
        public long? Id { get; set; }
        public long? TaskId { get; set; }
        public long? UserId { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedOn { get; set; }
        public IFormFile[] FileList { get; set; }
    }
}