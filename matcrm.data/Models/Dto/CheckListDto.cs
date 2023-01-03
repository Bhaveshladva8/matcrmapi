using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Dto
{
    public class CheckListDto
    {
        public CheckListDto()
        {
            AssignUsers = new List<CheckListAssignUserDto>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? TenantId { get; set; }
        public long? ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public int? UserId { get; set; }
        public bool IsChecked { get; set; }
        public bool IsDeleted { get; set; }
        public long? CheckListAssignUserId { get; set; }
        public List<CheckListAssignUserDto> AssignUsers { get; set; }
    }
}