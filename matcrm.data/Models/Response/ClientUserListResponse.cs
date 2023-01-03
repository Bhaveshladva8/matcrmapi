using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ClientUserListResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string WorkEmail { get; set; }
        public string Role { get; set; }
        public string WorkTelephoneNo { get; set; }
        public string LogoURL { get; set; }
    }
}