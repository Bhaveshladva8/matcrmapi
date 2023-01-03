using System;
using System.ComponentModel.DataAnnotations.Schema;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class OneClappFormLayoutDto
    {
        public OneClappFormLayoutDto(){
            LayoutBackground = new OneClappFormLayoutBackgroundDto();
        }
        public long? Id { get; set; }
        public bool TextDirectionRTL { get; set; }
        public long? LayoutBackgroundId { get; set; }
        public OneClappFormLayoutBackgroundDto LayoutBackground { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}