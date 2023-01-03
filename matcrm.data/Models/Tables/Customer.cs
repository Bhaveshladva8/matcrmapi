using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("Customer", Schema = "AppCRM")]
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column(TypeName = "varchar(1000)")]
        public string? Name { get; set; }
        // public long? LabelId { get; set; }

        // [ForeignKey ("LabelId")]
        // public virtual CustomerLabel CustomerLabel { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string? FirstName { get; set; }

        [Column(TypeName = "varchar(1000)")]
        public string? LastName { get; set; }
        public long? SalutationId { get; set; }
        [ForeignKey("SalutationId")]
        public virtual Salutation Salutation { get; set; }
        public long? OrganizationId { get; set; }
        public long? WeClappCustomerId { get; set; }
        public int? TenantId { get; set; }

        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}