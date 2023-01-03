using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("EmployeeTaskTimeRecord", Schema = "AppTask")]
    public class EmployeeTaskTimeRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public long? Duration { get; set; }
        public string Comment { get; set; }
        public bool IsBillable { get; set; }
        public long? EmployeeTaskId { get; set; }
        [ForeignKey("EmployeeTaskId")]
        public virtual EmployeeTask EmployeeTask { get; set; }
        public long? ServiceArticleId { get; set; }
        [ForeignKey("ServiceArticleId")]
        public virtual ServiceArticle ServiceArticle { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}