using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Dto {
    public class LabelDto {
        public LabelDto(){
            Categories = new List<string>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public long? LabelCategoryId { get; set; }
        public string CategoryName { get; set; }
         public List<string> Categories { get; set; }
        public int? TenantId { get; set; }
        public int? UserId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}