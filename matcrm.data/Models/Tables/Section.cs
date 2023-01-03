using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("Section", Schema = "AppTask")]
    public class Section
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Column(TypeName = "varchar(1500)")]
        public string? Name { get; set; }
        public long? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual EmployeeProject EmployeeProject { get; set; }
        public long? Priority { get; set; }
        public long? TenantId { get; set; }
        public long? TicketNumber { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}