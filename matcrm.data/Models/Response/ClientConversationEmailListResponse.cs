using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class ClientConversationEmailListResponse
    {
        //public string Email { get; set; }
        public int Totalcount { get; set; }
        public int Unreadcount { get; set; }
        public List<InboxThread> InboxThread { get; set; }
        public string NextPageToken { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
}