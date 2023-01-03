using System;
using System.ComponentModel.DataAnnotations;

namespace matcrm.data.Models.Dto
{
    public class EmailLogDto
    {
        public long? Id { get; set; }
        public string Body { get; set; }
        public bool Status { get; set; }

        [StringLength (254)]
        public string FromEmail { get; set; }

        [StringLength (254)]
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public long? TenantId { get; set; }

        [StringLength (5)]
        public string TemplateCode { get; set; }
        public string FromLabel { get; set; }
        public int? Tried { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}