using System;

namespace matcrm.data.Models.Response
{
    public class OrganizationNotesCommentAddUpdateResponse
    {
        public long? Id { get; set; }
        public long? OrganizationId { get; set; }
        public long? OrganizationNoteId { get; set; }
        public string Comment { get; set; }        
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        
    }
}