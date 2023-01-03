using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Dto {
    public class CustomerTypeDto {
        public long? Id { get; set; }

        [Column (TypeName = "varchar(n)")]
        public string Name { get; set; }
        public int? TenantId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}