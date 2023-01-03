using System;

namespace matcrm.data.Models.Request
{
    public class OrganizationNotesCommentDeleteRequest
    {
        public long? Id { get; set; }
        public long? OrganizationId { get; set; }        
    }
}