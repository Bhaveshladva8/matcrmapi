using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ClientInfoResponse
    {
        public ClientInfoResponse()
        {
            SocialMedias = new List<ClientDetailSocialMediaResponse>();            
        }
        public long Id { get; set; }
        public string ClientName { get; set; }
        public string LogoURL { get; set; }
        public string ClientUserName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string SiteName { get; set; }
        public long TaskCount { get; set; }
        public long ProjectCount { get; set; }
        public long ContractCount { get; set; }
        public long Fixed { get; set; }
        public long ConsumedFixed { get; set; }
        public long ConsumedHourly { get; set; }
        public List<ClientDetailSocialMediaResponse> SocialMedias { get; set; }
    }
    public class ClientDetailSocialMediaResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
    }
}