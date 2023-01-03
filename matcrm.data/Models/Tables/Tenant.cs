using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    public class Tenant {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int TenantId { get; set; }
        public string? TenantName { get; set; }
        public string? Username { get; set; }
        public string? Token { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? BlockedOn { get; set; }
        public int? BlockedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}