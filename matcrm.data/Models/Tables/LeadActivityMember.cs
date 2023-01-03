using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("LeadActivityMember", Schema = "AppCRM")]
    public class LeadActivityMember
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? LeadActivityId { get; set; }

        [ForeignKey("LeadActivityId")]
        public virtual LeadActivity LeadActivity { get; set; }

        [Column(TypeName = "varchar(254)")]
        public string? Email { get; set; }
    }
}