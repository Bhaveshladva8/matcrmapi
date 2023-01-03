using System;

namespace matcrm.data.Models.Dto
{
    public class EmailTemplateDto
    {
        public long EmailTemplateId { get; set; }
        public string TemplateName { get; set; }

        public string TemplateCode { get; set; }

        public string Description { get; set; }

        public string TemplateHtml { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}