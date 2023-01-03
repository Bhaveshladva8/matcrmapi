using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class CRMNotesAddUpdateRequest
    {
        public long? Id { get; set; }
        public string Note { get; set; }
        public long? ClientUserId { get; set; }        
        public long? Duration { get; set; }
        public bool? IsBillable { get; set; }
        public DateTime? NextCallDate { get; set; }
        public long? SatisficationLevelId { get; set; }
        public long? ClientId { get; set; }
        public long? MateTimeRecordId { get; set; }
    }
}