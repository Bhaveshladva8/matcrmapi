using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class CustomerNoteDeleteRequest
    {

        // public CustomerNoteDeleteRequest () {
        //     Comments = new List<CustomerNotesCommentDto> ();
        // }        
        public long? Id { get; set; }
        public long? CustomerId { get; set; }
        // public List<CustomerNotesCommentDto> Comments { get; set; }
        // public int? TenantId { get; set; }
        // public string Note { get; set; }
        // public bool IsDeleted { get; set; }
        // public int? CreatedBy { get; set; }
        // public DateTime? CreatedOn { get; set; }   
    }
}