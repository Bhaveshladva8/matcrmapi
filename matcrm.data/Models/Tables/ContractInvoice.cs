using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    public class ContractInvoice
    {
        public long Id { get; set; }
        public long? ClientInvoiceId { get; set; }
        [ForeignKey("ClientInvoiceId")]
        public virtual ClientInvoice ClientInvoice { get; set; } 
        public long? ContractId { get; set; }
        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}