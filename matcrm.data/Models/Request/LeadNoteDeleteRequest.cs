using System;

namespace matcrm.data.Models.Request
{
    public class LeadNoteDeleteRequest
    {
        public long? Id { get; set; }
        public long? LeadId { get; set; }        
       
    }
}