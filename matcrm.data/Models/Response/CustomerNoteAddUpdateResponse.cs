using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Response
{
    public class CustomerNoteAddUpdateResponse
    {
        public long? Id { get; set; }
        public long? CustomerId { get; set; }
        public string Note { get; set; }        
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }              
    }
}