using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Dto {
    public class CustomerNoteDto {
        public CustomerNoteDto () {
            Comments = new List<CustomerNotesCommentDto> ();
        }
        public long? Id { get; set; }
        public string Note { get; set; }
        public long? CustomerId { get; set; }
        public int? TenantId { get; set; }
        public List<CustomerNotesCommentDto> Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ShortName { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}