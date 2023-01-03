using System;

namespace matcrm.data.Models.Dto
{
    public class EmployeeTaskCommentDto
    {
        public long? Id { get; set; }
        public long? EmployeeTaskId { get; set; }
        public int? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Comment { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}