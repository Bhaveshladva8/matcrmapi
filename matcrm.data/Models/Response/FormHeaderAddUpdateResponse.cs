using System;

namespace matcrm.data.Models.Response
{
    public class FormHeaderAddUpdateResponse
    {
        public long? Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsEnabled { get; set; }
        public string CustomHeaderImage { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}