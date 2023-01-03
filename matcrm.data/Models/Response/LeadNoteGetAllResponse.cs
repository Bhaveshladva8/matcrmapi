using System;

namespace matcrm.data.Models.Response
{
    public class LeadNoteGetAllResponse
    {
        public long? Id { get; set; }
        public string Note { get; set; }
        public long? LeadId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ShortName { get; set; }        
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }        
    }
}