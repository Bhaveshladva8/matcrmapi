using System;

namespace matcrm.data.Models.Request
{
    public class OrganizationNoteAddUpdateRequest
    {
        public long? Id { get; set; }
        public string Note { get; set; }
        public long? OrganizationId { get; set; }        
    }
}