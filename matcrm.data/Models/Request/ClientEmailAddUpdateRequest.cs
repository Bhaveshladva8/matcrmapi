using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class ClientEmailAddUpdateRequest
    {
        public long? Id { get; set; }
        public string Email { get; set; }
        public long? EmailTypeId { get; set; }        
        public bool IsPrimary { get; set; }
        //public bool IsDelete { get; set; }
    }
}