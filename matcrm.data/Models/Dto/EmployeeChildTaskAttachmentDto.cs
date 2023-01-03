using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class EmployeeChildTaskAttachmentDto
    {
        public long Id { get; set; }
        public long? EmployeeChildTaskId { get; set; }
        public virtual EmployeeChildTask EmployeeChildTask { get; set; }
        public int? UserId { get; set; }
        public virtual User User { get; set; }
        public string? Name { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public IFormFile[] FileList { get; set; }
    }
}