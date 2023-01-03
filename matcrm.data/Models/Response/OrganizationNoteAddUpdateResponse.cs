using System;

namespace matcrm.data.Models.Response
{
    public class OrganizationNoteAddUpdateResponse
    {
        public long? Id { get; set; }
        public string Note { get; set; }
        public long? OrganizationId { get; set; }        
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }        
    }
}