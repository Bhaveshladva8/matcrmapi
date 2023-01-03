using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("ClientPhone", Schema = "AppCRM")]
    public class ClientPhone
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string PhoneNo { get; set; }
        public long? PhoneNoTypeId { get; set; }
        [ForeignKey("PhoneNoTypeId")]
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