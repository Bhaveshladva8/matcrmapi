using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    [Table("OrganizationActivity", Schema = "AppCRM")]
    public class OrganizationActivity {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column (TypeName = "varchar(250)")]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public string? Address { get; set; }
        public long? OrganizationId { get; set; }

        [ForeignKey ("OrganizationId")]
        public virtual Organization Organization { get; set; }
        public long? OrganizationActivityTypeId { get; set; }

        [ForeignKey ("OrganizationActivityTypeId")]
        public virtual ActivityType ActivityType { get; set; }
        public long? OrganizationActivityAvailabilityId { get; set; }

        [ForeignKey ("OrganizationActivityAvailabilityId")]
        public virtual ActivityAvailability ActivityAvailability { get; set; }
        public DateTime? ScheduleStartDate { get; set; }
        public DateTime? ScheduleEndDate { get; set; }

        [Column (TypeName = "varchar(150)")]
        public string? StartTime { get; set; }

        [Column (TypeName = "varchar(150)")]
        public string? EndTime { get; set; }
        public bool IsDone { get; set; }
        public int? TenantId { get; set; }

        [ForeignKey ("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}