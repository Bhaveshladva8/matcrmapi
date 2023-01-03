using System;
namespace matcrm.data.Models.Dto
{
    public class OneClappTaskUserDto
    {
        public long Id { get; set; }
        public int? UserId { get; set; }
        public long? OneClappTaskId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? TenantId { get; set; }
    }
}