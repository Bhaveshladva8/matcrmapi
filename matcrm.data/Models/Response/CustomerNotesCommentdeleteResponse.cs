using System;

namespace matcrm.data.Models.Response
{
    public class CustomerNotesCommentdeleteResponse
    {
        public long? Id { get; set; }
        public long? CustomerId { get; set; }
        public string Comment { get; set; }
        public long? CustomerNoteId { get; set; }       
        
    }
}