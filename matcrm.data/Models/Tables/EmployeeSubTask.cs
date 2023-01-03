using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("EmployeeSubTask", Schema = "AppTask")]
    public class EmployeeSubTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? WeClappTimeRecordId { get; set; }
        public long? EmployeeTaskId { get; set; }
        [ForeignKey("EmployeeTaskId")]
        public virtual EmployeeTask EmployeeTask { get; set; }
        public long? StatusId { get; set; }
        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}