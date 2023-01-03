using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Dto
{
    public class CheckListAssignUserDto
    {
        public CheckListAssignUserDto(){
            SelectedCheckList = new List<CheckListDto>();
        }
        public long? Id { get; set; }
        public long? CheckListId { get; set; }
        public int? TenantId { get; set; }
        public int? AssignUserId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string ShortName { get; set; }
        public List<CheckListDto> SelectedCheckList { get; set; }
    }
}