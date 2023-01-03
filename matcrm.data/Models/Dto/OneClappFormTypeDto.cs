using System;

namespace matcrm.data.Models.Dto
{
    public class OneClappFormTypeDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}