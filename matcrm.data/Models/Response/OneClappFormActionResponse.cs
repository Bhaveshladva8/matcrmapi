using System;

namespace matcrm.data.Models.Response
{
    public class OneClappFormActionResponse
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        
    }
}