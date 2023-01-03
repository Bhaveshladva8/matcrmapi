using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class IntProviderAppDto
    {
        public long? Id { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
        public long? IntProviderId { get; set; }
        [ForeignKey("IntProviderId")]
        public virtual IntProvider IntProvider { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}