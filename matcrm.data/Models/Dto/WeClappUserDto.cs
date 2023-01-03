using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class WeClappUserDto
    {
        public long? Id { get; set; }
        [StringLength(1000)]
        public string TenantName { get; set; }
        [StringLength(1500)]
        public string ApiKey { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}