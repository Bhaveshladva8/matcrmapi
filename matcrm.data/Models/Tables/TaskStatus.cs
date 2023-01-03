using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    [Table("TaskStatus", Schema = "AppTask")]
    public class TaskStatus {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column (TypeName = "varchar(500)")]
        public string? Name { get; set; }
        public int? TenantId { get; set; }
        public int? UserId { get; set; }
        [Column (TypeName = "varchar(200)")]
        public string? Color { get; set; }
        public bool IsFinalize { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}