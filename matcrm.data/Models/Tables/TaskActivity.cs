using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("TaskActivity", Schema = "AppTask")]
    public class TaskActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? TaskId { get; set; }
        public long? SectionId { get; set; }
        public long? UserId { get; set; }

        [Column(TypeName = "varchar(1000)")]
        public string? Activity { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}