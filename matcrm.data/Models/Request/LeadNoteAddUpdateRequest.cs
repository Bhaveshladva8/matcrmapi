using System;

namespace matcrm.data.Models.Request
{
    public class LeadNoteAddUpdateRequest
    {
        public long? Id { get; set; }
        public bool isEdit { get; set; }
        public long? LeadId { get; set; }
        public string Note { get; set; }
        
        
    }
}