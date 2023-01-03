using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    [Table("CustomerActivityMember", Schema = "AppCRM")]
    public class CustomerActivityMember {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? CustomerActivityId { get; set; }

        [ForeignKey ("CustomerActivityId")]
        public virtual CustomerActivity CustomerActivity { get; set; }

        [Column (TypeName = "varchar(254)")]
        public string? Email { get; set; }

    }
}