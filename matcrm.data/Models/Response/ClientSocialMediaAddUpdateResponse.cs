using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ClientSocialMediaAddUpdateResponse
    {
        public int Id { get; set; }
        public long ClientId { get; set; }
        public long SocialMediaId { get; set; }
        public string URL { get; set; }
    }
}