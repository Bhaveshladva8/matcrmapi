using System;
using System.Collections.Generic;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto {
    public class OneClappChildTaskDto {
        public long? Id { get; set; }
        public long? WeClappTimeRecordId { get; set; }
        public long? OneClappSubTaskId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public string Description { get; set; }
        public double? StartAt { get; set; }
        public string Ticket { get; set; }
        public double? Duration { get; set; }
        public string ApiKey { get; set; }
        public string Tenant { get; set; }
        public int? UserId { get; set; }
        public int? TenantId { get; set; }
        public int? StatusId { get; set; }
        public List<OneClappChildTaskUser> AssignedUsers { get; set; }
    }
}