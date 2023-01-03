using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    public class CalendarSubTask {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Column (TypeName = "varchar(500)")]
        public string? Title { get; set; }
        public string? Detail { get; set; }
        public long? CalendarTaskId { get; set; }

        [ForeignKey ("CalendarTaskId")]
        public virtual CalendarTask CalendarTask { get; set; }
        public DateTime? StartDate { get; set; }
        [Column (TypeName = "varchar(500)")]
        public string? StartTime { get; set; }
        public int? RepeatCount { get; set; }
        public long? RepeatTypeId { get; set; }
         [ForeignKey ("RepeatTypeId")]
        public virtual CalendarRepeatType CalendarRepeatType { get; set; }
        public bool IsDone { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}