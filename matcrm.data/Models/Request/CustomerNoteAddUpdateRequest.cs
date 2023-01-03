using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Request
{
    public class CustomerNoteAddUpdateRequest
    {
        public long? Id { get; set; }
        public long? CustomerId { get; set; }
        public string Note { get; set; }        
    }
}