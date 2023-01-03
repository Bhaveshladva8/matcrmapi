using System;

namespace matcrm.data.Models.Dto
{
    public class OneClappLatestThemeLayoutDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string TemplateHtml { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}