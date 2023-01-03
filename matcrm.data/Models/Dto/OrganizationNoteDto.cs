using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Dto {
    public class OrganizationNoteDto {
        public OrganizationNoteDto () {
            Comments = new List<OrganizationNotesCommentDto> ();
        }
        public long? Id { get; set; }
        public string Note { get; set; }
        public long? OrganizationId { get; set; }
        public List<OrganizationNotesCommentDto> Comments { get; set; }
        public int? TenantId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ShortName { get; set; }
    }
}