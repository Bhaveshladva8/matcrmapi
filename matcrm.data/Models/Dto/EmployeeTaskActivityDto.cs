using System;

namespace matcrm.data.Models.Dto
{
    public class EmployeeTaskActivityDto
    {
        public long? Id { get; set; }
        public long? EmployeeTaskId { get; set; }
        public long? UserId { get; set; }
        public long? ProjectId { get; set; }
        public string Activity { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ShortName { get; set; }
    }
}