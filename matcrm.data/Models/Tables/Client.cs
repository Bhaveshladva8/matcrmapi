using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("Client", Schema = "AppCRM")]
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string Name { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string FirstName { get; set; }
        [Column(TypeName = "varchar(500)")]
        public string LastName { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string? OrganizationName { get; set; }

        [Column(TypeName = "varchar(1000)")]
        public string SiteName { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string SiteContactNumber { get; set; }

        [Column(TypeName = "varchar")]
        public string SiteAddressLine1 { get; set; }

        [Column(TypeName = "varchar")]
        public string SiteAddressLine2 { get; set; }
        [Column(TypeName = "varchar")]
        public string SiteAddressLine3 { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string PostalCode { get; set; }
        public long? CountryId { get; set; }
        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }
        public long? StateId { get; set; }
        [ForeignKey("StateId")]
        public virtual State State { get; set; }
        public long? CityId { get; set; }
        [ForeignKey("CityId")]
        public virtual City City { get; set; }
        public long? TimeZoneId { get; set; }
        [ForeignKey("TimeZoneId")]
        public virtual StandardTimeZone StandardTimeZone { get; set; }
        public bool IsActive { get; set; }
        public string? Logo { get; set; }
        public long? InvoiceIntervalId { get; set; }
        [ForeignKey("InvoiceIntervalId")]
        public virtual InvoiceInterval InvoiceInterval { get; set; }
        public bool IsContractBaseInvoice { get; set; }
        public int? TenantId { get; set; }

        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}