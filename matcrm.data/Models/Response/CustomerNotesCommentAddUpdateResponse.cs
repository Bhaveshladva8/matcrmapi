using System;

namespace matcrm.data.Models.Response
{
    public class CustomerNotesCommentAddUpdateResponse
    {
        public long? Id { get; set; }
        public long? CustomerId { get; set; }
        public long? CustomerNoteId { get; set; }
        public string Comment { get; set; }        
        public DateTime? CreatedOn { get; set; }        
    }
}