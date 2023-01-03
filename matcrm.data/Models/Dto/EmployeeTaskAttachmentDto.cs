using System;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Dto
{
    public class EmployeeTaskAttachmentDto
    {
        public long? Id { get; set; }
        public long? EmployeeTaskId { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public IFormFile[] FileList { get; set; }
    }
}