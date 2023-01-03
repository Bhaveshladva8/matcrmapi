using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ClientSocialMediaListResponse
    {
        public int Id { get; set; }
        public long? SocialMediaId { get; set; }
        public string URL { get; set; }
    }
}