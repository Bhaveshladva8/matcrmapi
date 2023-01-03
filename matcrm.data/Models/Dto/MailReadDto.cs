using System;

namespace matcrm.data.Models.Dto
{
    public class MailReadDto
    {
        public long? Id { get; set; }
        public string? ThreadId { get; set; }
        public int? ReadBy { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}