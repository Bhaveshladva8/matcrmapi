using System;

namespace matcrm.data.Models.Response
{
    public class LabelDeleteResponse
    {        
        public long Id { get; set; }        
        public string? Name { get; set; }      
        public string? Color { get; set; }
        public long? LabelCategoryId { get; set; }        
    }
}