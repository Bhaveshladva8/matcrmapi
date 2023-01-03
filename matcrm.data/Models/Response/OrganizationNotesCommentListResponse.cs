using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class OrganizationNotesCommentListResponse
    {
        public long? Id { get; set; }
        public long? OrganizationId { get; set; }
        public long? OrganizationNoteId { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }            
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ShortName { get; set; }
    }
}