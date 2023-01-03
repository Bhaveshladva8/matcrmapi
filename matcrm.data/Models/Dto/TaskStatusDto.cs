using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Dto {
    public class TaskStatusDto {
         public TaskStatusDto (){
            CustomFields = new List<CustomFieldDto>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int? TenantId { get; set; }
        public int? UserId { get; set; }
        public bool IsFinalize { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}