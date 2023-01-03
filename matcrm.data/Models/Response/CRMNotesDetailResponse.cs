using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class CRMNotesDetailResponse
    {
        public long Id { get; set; }
        public string Note { get; set; }
        public string LogoURL { get; set; }
        public long? ClientUserId { get; set; }
        public string ClientUserName { get; set; }
        public long? MateTimeRecordId { get; set; }
        public long? Duration { get; set; }
        public bool? IsBillable { get; set; }
        public DateTime? NextCallDate { get; set; }
        public long? SatisficationLevelId { get; set; }
        public string SatisficationLevel { get; set; }
    }
}