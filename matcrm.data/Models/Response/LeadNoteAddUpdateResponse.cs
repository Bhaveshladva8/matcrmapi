using System;

namespace matcrm.data.Models.Response
{
    public class LeadNoteAddUpdateResponse
    {        
        public long? Id { get; set; }        
        public long? LeadId { get; set; }
        public string Note { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        
    }
}