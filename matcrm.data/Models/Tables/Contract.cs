using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    public class Contract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public bool IsBillingFromStartDate { get; set; }
        public long? Discount { get; set; }
        public long? ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        public long? ContractTypeId { get; set; }
        [ForeignKey("ContractTypeId")]
        public virtual ContractType ContractType { get; set; }
        public long? Amount { get; set; }
        public long? DefaultUnitPrice { get; set; }
        public long? CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }
        public long? InvoiceIntervalId { get; set; }
        [ForeignKey("InvoiceIntervalId")]
        public virtual InvoiceInterval InvoiceInterval { get; set; }
        public long? StatusId { get; set; }
        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; }
        public long? AllowedHours { get; set; }
        public int? CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User CreatedUser { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedUser { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? DeletedBy { get; set; }
        [ForeignKey("DeletedBy")]
        public virtual User DeletedUser { get; set; }
        public DateTime? DeletedOn { get; set; }

    }
}