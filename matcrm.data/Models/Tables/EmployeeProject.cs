using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("EmployeeProject", Schema = "AppTask")]
    public class EmployeeProject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Column(TypeName = "varchar(1500)")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Logo { get; set; }
        public DateTime? StartDate { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{hh:mm:ss}", ApplyFormatInEditMode = true)]
        public TimeSpan? EstimatedTime { get; set; }
        public DateTime? EndDate { get; set; }

        public long? ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }

        public long? Priority { get; set; }
        public long? StatusId { get; set; }
        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; }
        // public virtual EmployeeProjectStatus EmployeeProjectStatus { get; set; }
        public int? TenantId { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public long? MateCategoryId { get; set; }
        [ForeignKey("MateCategoryId")]
        public virtual MateCategory MateCategory { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}