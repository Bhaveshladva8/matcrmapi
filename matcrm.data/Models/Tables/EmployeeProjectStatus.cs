using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("EmployeeProjectStatus", Schema = "AppTask")]
    public class EmployeeProjectStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Column(TypeName = "varchar(500)")]
        public string? Name { get; set; }
        public int? TenantId { get; set; }
        public int? UserId { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string? Color { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}