using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    public class WeClappUser
    {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Column (TypeName = "varchar(1000)")]
        public string? TenantName { get; set; }
        [Column (TypeName = "varchar(1500)")]
        public string? ApiKey { get; set; }
        public int? UserId { get; set; }
        [ForeignKey ("UserId")]
        public virtual User User { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }

    }
}