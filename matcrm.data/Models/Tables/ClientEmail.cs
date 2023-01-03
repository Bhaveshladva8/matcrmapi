using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("ClientEmail", Schema = "AppCRM")]
    public class ClientEmail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string Email { get; set; }
        public long? EmailTypeId { get; set; }
        [ForeignKey("EmailTypeId")]
        public virtual EmailPhoneNoType EmailPhoneNoType { get; set; }
        public long? ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        public bool IsPrimary { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }        
        public DateTime? DeletedOn { get; set; }
    }
}