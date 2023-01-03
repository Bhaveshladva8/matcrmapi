using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Dto
{
    public class OneClappFormStatusDto
    {
        public long? Id { get; set; }

        [StringLength(500)]
        public string Name { get; set; }
        public int? TenantId { get; set; }
        public int? CreatedBy { get; set; }
        [StringLength(200)]
        public string Color { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}