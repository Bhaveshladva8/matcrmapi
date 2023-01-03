using System;

namespace matcrm.data.Models.Dto
{
    public class EmployeeTaskTimeRecordDto
    {
        public long? Id { get; set; }
        public int? UserId { get; set; }
        public long? Duration { get; set; }
        public long? EmployeeTaskId { get; set; }
        public long? ServiceArticleId { get; set; }
        public string Comment { get; set; }
        public bool IsBillable { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}