using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Dto
{
    public class ChildTaskAttachmentDto
    {
        public long? Id { get; set; }
        public long? ChildTaskId { get; set; }
        public long? UserId { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedOn { get; set; }
        public IFormFile[] FileList { get; set; }
    }
}