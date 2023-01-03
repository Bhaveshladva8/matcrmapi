using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class OrganizationNoteGetAllRespnse
    {
        public OrganizationNoteGetAllRespnse()
        {
            Comments = new List<OrganizationNotesCommentDto>();
        }
        public long? Id { get; set; }
        public string Note { get; set; }
        public long? OrganizationId { get; set; }
        public List<OrganizationNotesCommentDto> Comments { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ShortName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }


    }
}