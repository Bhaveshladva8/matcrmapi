using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ClientUserContactsList
    {
        public string Id { get; set; }
        public string FirstName { get; set; }        
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsSameDomain { get; set; }        
        //public long? ClientId { get; set; }
        
    }
}