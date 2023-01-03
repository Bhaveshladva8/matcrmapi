using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ClientUserCreateContactResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        //public string CreatedOn { get; set; }
        public string error_description { get; set; }
    }
}