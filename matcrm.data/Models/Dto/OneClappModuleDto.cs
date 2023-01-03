using System;
using System.ComponentModel.DataAnnotations;

namespace matcrm.data.Models.Dto
{
    public class OneClappModuleDto
    {
        public long? Id { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}