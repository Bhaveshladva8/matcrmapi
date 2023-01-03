using System;

namespace matcrm.data.Models.Dto
{
    public class SectionActivityDto
    {
        public long? Id { get; set; }
        public long? SectionId { get; set; }
        public long? UserId { get; set; }
        public string Activity { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}