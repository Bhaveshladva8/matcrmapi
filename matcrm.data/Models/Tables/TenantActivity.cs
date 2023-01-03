using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    public class TenantActivity {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? TenantId { get; set; }
        public long? UserId { get; set; }

        [Column (TypeName = "varchar(1000)")]
        public string? Activity { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}