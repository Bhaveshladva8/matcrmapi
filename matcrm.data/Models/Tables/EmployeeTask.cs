using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("EmployeeTask", Schema = "AppTask")]
    public class EmployeeTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? StatusId { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; }
        // public virtual EmployeeTaskStatus EmployeeTaskStatus { get; set; }
        public int? TenantId { get; set; }

        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? Priority { get; set; }                
        public long? SectionId { get; set; }
        [ForeignKey("SectionId")]
        public virtual Section Section { get; set; }
        public long? MatePriorityId { get; set; }

        [ForeignKey("MatePriorityId")]
        public virtual MatePriority MatePriority { get; set; }
        public string TaskNo { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string Name { get; set; }        
    }
}