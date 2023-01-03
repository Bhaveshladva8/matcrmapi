using System;

namespace matcrm.data.Models.Dto {
    public class OrganizationNotesCommentDto {
        public long? Id { get; set; }
        public long? OrganizationId { get; set; }
        public long? OrganizationNoteId { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ShortName { get; set; }
    }
}