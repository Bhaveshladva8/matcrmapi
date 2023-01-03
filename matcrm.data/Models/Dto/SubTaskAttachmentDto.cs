using System;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Dto {
    public class SubTaskAttachmentDto {
        public long? Id { get; set; }
        public long? SubTaskId { get; set; }
        public long? UserId { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedOn { get; set; }
        public IFormFile[] FileList { get; set; }
    }
}