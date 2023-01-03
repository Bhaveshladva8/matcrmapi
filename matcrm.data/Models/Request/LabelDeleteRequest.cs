using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Request
{
    public class LabelDeleteRequest
    {        
        public long? Id { get; set; }       
        
        public string CategoryName { get; set; }        
    }
}