using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    public class InvoiceMollieSubscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        public long? ClientInvoiceId { get; set; }
        [ForeignKey("ClientInvoiceId")]
        public virtual ClientInvoice ClientInvoice { get; set; }
        public string? SubscriptionId { get; set; }
        public string? PaymentId { get; set; }
        public string? Status { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}