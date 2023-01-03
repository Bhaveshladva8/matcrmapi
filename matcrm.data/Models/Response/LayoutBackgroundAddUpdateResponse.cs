using System;

namespace matcrm.data.Models.Response
{
    public class LayoutBackgroundAddUpdateResponse
    {
        public long? Id { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CustomBackgroundImage { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}