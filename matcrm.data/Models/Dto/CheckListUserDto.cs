using System;

namespace matcrm.data.Models.Dto
{
    public class CheckListUserDto
    {
        public long? Id { get; set; }
        public long? CheckListId { get; set; }
        public int? TenantId { get; set; }
        public bool IsChecked { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}