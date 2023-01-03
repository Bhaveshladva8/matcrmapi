using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class ClientConversationEmailListRequest
    {
        public long ClientId { get; set; }
        public string Label { get; set; }
        public int? page { get; set; }
        public int? skip { get; set; }
        public int? top { get; set; }
        public string nextPageToken { get; set; }
    }
}