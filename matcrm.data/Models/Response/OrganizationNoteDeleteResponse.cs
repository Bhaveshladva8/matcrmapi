using System;

namespace matcrm.data.Models.Response
{
    public class OrganizationNoteDeleteResponse
    {
        public long Id { get; set; }
        public string? Note { get; set; }
        public long? OrganizationId { get; set; }        
    }
}