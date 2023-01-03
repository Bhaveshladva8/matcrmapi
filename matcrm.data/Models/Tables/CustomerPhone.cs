using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    [Table("CustomerPhone", Schema = "AppCRM")]
    public class CustomerPhone {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column (TypeName = "varchar(50)")]
        public string? PhoneNo { get; set; }
        public long? PhoneNoTypeId { get; set; }

        [ForeignKey ("PhoneNoTypeId")]
        public virtual EmailPhoneNoType EmailPhoneNoType { get; set; }
        public long? CustomerId { get; set; }

        [ForeignKey ("CustomerId")]
        public virtual Customer Customer { get; set; }
        public int? TenantId { get; set; }

        [ForeignKey ("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}