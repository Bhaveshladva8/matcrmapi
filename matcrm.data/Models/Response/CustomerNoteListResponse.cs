using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class CustomerNoteListResponse
    {
        public CustomerNoteListResponse () {
            Comments = new List<CustomerNotesCommentDto> ();
        }
        public long? Id { get; set; }
        public string Note { get; set; }
        public long? CustomerId { get; set; }        
        public List<CustomerNotesCommentDto> Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ShortName { get; set; }        
    }
}