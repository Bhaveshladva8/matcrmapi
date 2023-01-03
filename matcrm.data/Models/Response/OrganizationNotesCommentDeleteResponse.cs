using System;

namespace matcrm.data.Models.Response
{
    public class OrganizationNotesCommentDeleteResponse
    {
        public long? Id { get; set; }
        public long? OrganizationId { get; set; }        
        public long? OrganizationNoteId { get; set; }
        public string Comment { get; set; }
        
    }
}