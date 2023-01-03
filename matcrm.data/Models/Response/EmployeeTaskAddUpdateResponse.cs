using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Request;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Response
{
    public class EmployeeTaskAddUpdateResponse
    {
        // public long? Id { get; set; }
        // public long? StatusId { get; set; }
        // public int? TenantId { get; set; }
        // public int? UserId { get; set; }
        // public bool IsActive { get; set; }
        // public string Description { get; set; }
        // public DateTime? StartDate { get; set; }
        // public DateTime? EndDate { get; set; }
        // public long? Priority { get; set; }
        // public long? ProjectId { get; set; }
        // public long? CreatedBy { get; set; }
        // public DateTime? CreatedOn { get; set; }
        // public long? SectionId { get; set; }
        // public List<EmployeeTaskUserRequestResponse> AssignedUsers { get; set; }

        public long? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? ProjectId { get; set; }       
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }        
        public long? StatusId { get; set; }
        public List<EmployeeTaskUserRequestResponse> AssignedUsers { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}