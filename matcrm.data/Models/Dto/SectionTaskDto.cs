using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Dto
{
    public class SectionTaskDto
    {
        public string Id { get; set; }
        public long? ProjectId { get; set; }
        public string CustomerId { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double CreatedDate { get; set; }
        public bool Billable { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public string Subject { get; set; }
        public bool DisableEmailTemplates { get; set; }
        public string AssignedUserId { get; set; }
        public string AssignedUserUsername { get; set; }
        public string Message { get; set; }
        public string Username { get; set; }
        public string ApiKey { get; set; }
        public string Tenant { get; set; }
        public long? TenantId { get; set; }
        public string FilterTaskDescription { get; set; }
        public int? StatusId { get; set; }
        public string ShortColumn { get; set; }
        public string ShortOrder { get; set; }
    }

    public class SectionTaskWithoutProjectRequestDto
    {
        public string Description { get; set; }
        public string FilterTaskDescription { get; set; }
        public int? StatusId { get; set; }
        public string ShortColumn { get; set; }
        public string ShortOrder { get; set; }
    }
}