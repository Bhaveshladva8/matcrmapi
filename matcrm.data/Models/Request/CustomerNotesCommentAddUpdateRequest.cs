using System;

namespace matcrm.data.Models.Request
{
    public class CustomerNotesCommentAddUpdateRequest
    {
        public long? Id { get; set; }
        public string Comment { get; set; }
        public long? CustomerId { get; set; }
        public long? CustomerNoteId { get; set; }        
    }
}