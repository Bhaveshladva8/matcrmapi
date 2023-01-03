using System;

namespace matcrm.data.Models.Dto {
    public class TableColumnUserDto {
        public long? Id { get; set; }

        // In which table you want to sort column
        public long? MasterTableId { get; set; }

        public long? TableColumnId { get; set; }
        public int? UserId { get; set; }
        public long? Priority { get; set; }
        public Boolean IsHide { get; set; }
        public int? TenantId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}