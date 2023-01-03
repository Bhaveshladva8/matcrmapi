using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("OrganizationActivityMember", Schema = "AppCRM")]
    public class OrganizationActivityMember
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? OrganizationActivityId { get; set; }

        [ForeignKey("OrganizationActivityId")]
        public virtual OrganizationActivity OrganizationActivity { get; set; }

        [Column(TypeName = "varchar(254)")]
        public string? Email { get; set; }
    }
}