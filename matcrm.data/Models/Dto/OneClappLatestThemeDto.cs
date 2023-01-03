using System;

namespace matcrm.data.Models.Dto
{
    public class OneClappLatestThemeDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Accent { get; set; }
        public string Primary { get; set; }
        public string Warn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}