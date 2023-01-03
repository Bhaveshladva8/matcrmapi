using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Tables
{
    [Table("CRMNotes", Schema = "AppCRM")]
    public class CRMNotes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Note { get; set; }
        public long? ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        public long? ClientUserId { get; set; }
        [ForeignKey("ClientUserId")]
        public virtual ClientUser ClientUser { get; set; }
        public long? MateTimeRecordId { get; set; }
        [ForeignKey("MateTimeRecordId")]
        public virtual MateTimeRecord MateTimeRecord { get; set; }
        public DateTime? NextCallDate { get; set; }
        public long? SatisficationLevelId { get; set; }
        [ForeignKey("SatisficationLevelId")]
        public virtual SatisficationLevel SatisficationLevel { get; set; }
        public int? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual User CreatedUser { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedUser { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }

    }
}