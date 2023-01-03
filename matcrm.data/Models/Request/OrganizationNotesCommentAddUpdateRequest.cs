using System;

namespace matcrm.data.Models.Request
{
    public class OrganizationNotesCommentAddUpdateRequest
    {
        public long? Id { get; set; }
        public string Comment { get; set; }
        public long? OrganizationId { get; set; }
        public long? OrganizationNoteId { get; set; }        
    }
}